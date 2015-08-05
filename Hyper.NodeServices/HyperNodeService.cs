using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Hyper.ActivityTracking;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.Client;
using Hyper.NodeServices.CommandModules;
using Hyper.NodeServices.CommandModules.SystemCommands;
using Hyper.NodeServices.CommandModules.TestCommands;
using Hyper.NodeServices.Configuration;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices
{
    /// <summary>
    /// This class is the heart of the concept of a "HyperNode," which is able to process <see cref="HyperNodeMessageRequest"/> objects and return <see cref="HyperNodeMessageResponse"/> objects.
    /// When a <see cref="HyperNodeService"/> object receives a message, it will choose to either ignore it or process it. Details about what happened during message processing such as
    /// why the message was ignored or whether there were any errors during processing are contained in the response object. The following are some possible results:
    /// 
    /// 1) The message was ignored. This could happen for one of the the following reasons:
    ///    - The message itself had expired
    ///    - This specific <see cref="HyperNodeService"/> object was not one of the intended recipients of the message
    ///    - This specific <see cref="HyperNodeService"/> object has already processed the message
    /// 2) The message failed. This could happen for one of the following reasons:
    ///    - This specific <see cref="HyperNodeService"/> object did not recognize the message and was unable to determine how to process it.
    ///    - This specific <see cref="HyperNodeService"/> object was an intended recipient, recognized the message, and acknowledged the ability to process the message, but encountered
    ///      a fatal error during processing.
    /// 3) The message was processed successfully, possibly with warnings and/or non-fatal errors. The following flags may be specified in any combination via bitwise OR: 
    ///    - Success
    ///    - HadNonFatalErrors
    ///    - HadWarnings
    /// </summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single
    )]
    public sealed class HyperNodeService : IHyperNodeService, IDisposable
    {
        #region Private Members

        private readonly string _hyperNodeName;
        private readonly object _lock = new object();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private static readonly ITaskIdProvider DefaultTaskIdProvider = new GuidTaskIdProvider();
        private static readonly ICommandRequestSerializer DefaultRequestSerializer = new PassThroughSerializer();
        private static readonly ICommandResponseSerializer DefaultResponseSerializer = new PassThroughSerializer();
        private readonly TimeSpan _defaultActivityCacheSlidingExpiration = TimeSpan.FromHours(1);
        private readonly ProgressCacheItemCollector _activityCache = new ProgressCacheItemCollector();
        private readonly List<HyperNodeServiceActivityMonitor> _activityMonitors = new List<HyperNodeServiceActivityMonitor>();
        private readonly ConcurrentDictionary<string, CommandModuleConfiguration> _commandModuleConfigurations = new ConcurrentDictionary<string, CommandModuleConfiguration>();

        /// <summary>
        /// This member is meant to store a backup of all of the <see cref="IDisposable" /> subscribers to our activity feed.
        /// If the <see cref="HyperNodeService"/> object is forced to be disposed while it still has child threads processing,
        /// the <see cref="ChildThreadCleanup" /> method may not be called properly. In this case, we would lose our only
        /// reference to the <see cref="IDisposable" /> subscribers waiting to be disposed, resulting in a memory leak. To
        /// mitigate this possibility, this backup reference is stored so that we have one last chance to clean up when the
        /// <see cref="HyperNodeService" /> object is disposed.
        /// </summary>
        private readonly List<IDisposable> _backupSubscriberReference = new List<IDisposable>();

        #endregion Private Members

        #region Properties

        private string HyperNodeName
        {
            get { return _hyperNodeName; }
        }

        private ITaskIdProvider TaskIdProvider { get; set; }

        private bool EnableActivityCache { get; set; }

        private TimeSpan ActivityCacheSlidingExpiration
        {
            set { _activityCache.CacheDuration = value; }
        }

        private static HyperNodeService _instance;
        public static HyperNodeService Instance
        {
            get { return _instance ?? (_instance = Create()); }
        }

        #endregion Properties

        #region Public Methods

        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
        {
            // TODO: Validate message contents and throw exceptions if invalid message?

            var response = new HyperNodeMessageResponse(this.HyperNodeName)
            {
                NodeAction = HyperNodeActionType.None,
                NodeActionReason = HyperNodeActionReasonType.Unknown,
                ProcessStatusFlags = MessageProcessStatusFlags.None
            };

            #region Create Task ID

            var taskId = "";
            HyperNodeActivityItem illBehavedTaskIdProviderActivityItem = null;

            try
            {
                // Try to use our custom task ID provider
                taskId = this.TaskIdProvider.CreateTaskId(message);

                // Check for a blank task ID
                if (string.IsNullOrWhiteSpace(taskId))
                {
                    illBehavedTaskIdProviderActivityItem = new HyperNodeActivityItem(this.HyperNodeName)
                    {
                        EventDescription = string.Format(
                            "The class '{0}' created a blank task ID.",
                            this.TaskIdProvider.GetType().FullName
                        )
                    };
                }
            }
            catch (Exception ex)
            {
                // Custom task ID provider threw an exception, so we'll just use our default provider to recover
                illBehavedTaskIdProviderActivityItem = new HyperNodeActivityItem(this.HyperNodeName)
                {
                    EventDescription = "An exception was thrown while attempting to create a task ID.",
                    EventDetail = ex.ToString()
                };
            }

            // Check if we had an ill-behaved task ID provider. If so, use our default task ID provider instead.
            if (illBehavedTaskIdProviderActivityItem != null)
            {
                taskId = DefaultTaskIdProvider.CreateTaskId(message);

                illBehavedTaskIdProviderActivityItem.EventDescription += " The default task ID provider was used to generate a non-blank task ID instead.";
                response.TaskTrace.Add(illBehavedTaskIdProviderActivityItem);
            }

            // Finally, set our task ID
            response.TaskId = taskId;

            #endregion Create Task ID

            #region Activity Tracker Setup

            var activityTracker = new HyperNodeServiceActivityTracker(
                new HyperNodeActivityContext(
                    this.HyperNodeName,
                    message.MessageGuid,
                    message.CommandName,
                    response.TaskId
                )
            );

            /*****************************************************************************************************************
             * Setup our activity tracker
             * 
             * The activity tracker triggers activity tracking events for monitor objects that subscribe to the events. The
             * subscribers are split into two groups: those that should stick around after the current method call to report
             * on long-running child tasks, and those that should be disposed of at the end of this method call.
             *****************************************************************************************************************/
            var longLivedSubscribers = new CompositeDisposable(); // Container for subscribers that may need to live longer than this method call
            var shortLivedSubscribers = new CompositeDisposable(); // Container for subscribers that will be disposed at the end of this method call

            try
            {
                // Create our activity feed by bridging the event tracker with reactive extensions
                var liveEvents = Observable.FromEventPattern<TrackActivityEventHandler, TrackActivityEventArgs>(
                    h => activityTracker.TrackActivityHandler += h,
                    h => activityTracker.TrackActivityHandler -= h
                ).Select(
                    a => a.EventArgs.ActivityItem as IHyperNodeActivityEventItem // Cast all our activity items as IHyperNodeActivityEventItem
                );

                /*****************************************************************************************************************
                 * Subscribe our progress cache to our event stream only if the client requested it and the feature is actually enabled. There is currently no built-in
                 * functionality to support long-running task tracing other than the memory cache. If the client opts to disable the memory cache to save resources,
                 * then they will need to setup a custom HyperNodeServiceActivityMonitor if they still want to be able to know what's going on in the server. Custom
                 * HyperNodeServiceActivityMonitor objects can record activity to a database or some other data store, which the user can then query for the desired
                 * activity.
                 *****************************************************************************************************************/
                if (this.EnableActivityCache && message.CacheProgressInfo)
                {
                    longLivedSubscribers.Add(
                        liveEvents
                            .Where(a => _activityCache.ShouldTrack(a))
                            .Subscribe(_activityCache)
                    );
                }

                /*****************************************************************************************************************
                 * If we want a task trace returned, that's fine, but the task trace in the real-time response would only ever contain events recorded during the current
                 * call to ProcessMessage() as opposed to the entire lifetime of a task in general. This is because a task can be spawned in a child thread and can outlive
                 * the original call to ProcessMessage(). This is the whole reason why we have a ProgressCacheItemCollector that is in charge of collecting trace information
                 * for tasks that run concurrently. It should be noted that if the client elects to use caching AND return a task trace, then the events recorded in the
                 * real-time task trace and those recorded in the cache will likely overlap for events which fired before ProcessMessage() returned. However, any events that
                 * fired after ProcessMessage() returned would only be recorded in the cache. This may result in a task trace that looks incomplete since the "processing
                 * complete" message would not have occured before the method returned.
                 *****************************************************************************************************************/
                if (message.ReturnTaskTrace)
                {
                    var taskTraceCollector = new HyperNodeTaskTraceCollector(response);
                    longLivedSubscribers.Add(
                        liveEvents
                            .Where(a => taskTraceCollector.ShouldTrack(a))
                            .Subscribe(taskTraceCollector)
                    );
                }

                /*****************************************************************************************************************
                 * Concurrent tasks cannot return anything meaningful in the real-time response because the main thread gets to the return statement long before the
                 * child threads complete their processing. Any response that is returned by the main thread contains minimal information: perhaps an incomplete task
                 * trace and some enum values, but that's it. Therefore, by design, clients who elect to run tasks concurrently inherently decide to disregard the
                 * real-time response object in favor of a progress cache which can be queried after the initial call. This cache can either be the in-memory cache
                 * of the hypernode, or it can be some other repository that the user sets up themselves via a custom monitor.
                 * 
                 * On the other hand, if the client elects to run non-concurrently (aka synchronously), then it expects to and should receive a complete response
                 * object.
                 * 
                 * This response collector monitor's job is to collect response objects for child nodes to which the message is forwarded and add them to the target
                 * response object. Responses compiled this way have a tree-like structure in which child node responses are referenced by their node name.
                 * 
                 * For synchronous operations, this results in a real-time response that contains all of the response information for this node and its decendants.
                 * 
                 * For asynchronous operations, the real-time response contains the task ID for the main thread executed on this node. The cached response for that
                 * real-time task ID contains the task IDs of the main threads executed on the child nodes. The cached responses for those task IDs contain the
                 * task IDs of the main threads executed on the grandchildren nodes, etc. Perhaps a new command can be written to automatically get a list of all
                 * intended recipients and the corresponding "main" task IDs to use to request status updates. This would be exclusively for the cache. If the
                 * user wanted to use something other than the cache, they would have to determine the task IDs their own way.
                 *****************************************************************************************************************/
                var responseCollector = new HyperNodeMessageResponseCollector(response);
                longLivedSubscribers.Add(
                    liveEvents
                        .Where(a => responseCollector.ShouldTrack(a))
                        .Subscribe(responseCollector)
                );

                /*****************************************************************************************************************
                 * Subscribe our custom activity monitors to the event stream last, just in case any of them throw exceptions. If they do, we've already setup our
                 * task trace and cache monitors at this point, so we can actually use the activity tracker to track the exceptions. This way, we can make sure that
                 * any exceptions thrown by user code are available to be reported to the client
                 *****************************************************************************************************************/
                longLivedSubscribers.Add(
                    new CompositeDisposable(
                        from monitor in _activityMonitors
                        where monitor.Enabled
                        // Make sure the monitors are enabled
                        select liveEvents
                            .Where(
                                e =>
                                {
                                    var shouldTrack = false;

                                    try
                                    {
                                        // ...for activity items matching the specified criteria
                                        shouldTrack = monitor.ShouldTrack(e);
                                    }
                                    catch (Exception ex)
                                    {
                                        // This is legal at this point because we already setup our cache and task trace monitors earlier
                                        activityTracker.TrackException(
                                            new ActivityMonitorSubscriptionException(
                                                string.Format(
                                                    "Unable to subscribe monitor '{0}' because its ShouldTrack() method threw an exception.",
                                                    monitor.Name
                                                ),
                                                ex
                                            )
                                        );
                                    }

                                    return shouldTrack;
                                }
                            )
                            .ObserveOn(ThreadPoolScheduler.Instance) // Force custom activity monitors to run on the threadpool in case they are long-running and/or ill-behaved
                            .Subscribe(monitor)
                    )
                );
            }
            catch (Exception ex)
            {
                // This is a safe thing to do, it may just result in nothing being reported if Reactive Extensions wasn't setup properly
                activityTracker.TrackException(
                    new ActivityTrackerInitializationException(
                        "An exception was thrown while initializing the activity tracker. See inner exception for details.",
                        ex
                    )
                );
            }
            finally
            {
                // If we're running concurrently and we've successfully added some long-lived subscribers, be sure to backup the reference
                if (message.RunConcurrently && longLivedSubscribers.Count > 0)
                {
                    lock (_lock)
                    {
                        _backupSubscriberReference.Add(longLivedSubscribers);
                    }
                }
            }

            #endregion Activity Tracker Setup

            try
            {
                var childTasks = new List<Task>();

                // Confirm receipt of the message
                activityTracker.TrackReceived();

                if (message.ExpirationDateTime <= DateTime.Now)
                {
                    // Message has expired, so ignore it and do not forward it
                    response.NodeAction = HyperNodeActionType.Ignored;
                    response.NodeActionReason = HyperNodeActionReasonType.MessageExpired;
                    activityTracker.TrackIgnored("Message expired on " + message.ExpirationDateTime.ToString("G") + ".");
                }
                else if (message.SeenByNodeNames.Contains(this.HyperNodeName))
                {
                    // Message was already processed by this node, so ignore it and do not forward it (since it would have been forwarded the first time)
                    response.NodeAction = HyperNodeActionType.Ignored;
                    response.NodeActionReason = HyperNodeActionReasonType.PreviouslySeen;
                    activityTracker.TrackIgnored("Message previously seen.");
                }
                else
                {
                    // Add this node to the list of nodes who have seen this message
                    message.SeenByNodeNames.Add(this.HyperNodeName);
                    activityTracker.TrackSeen();

                    // Check if this message was intended for this node
                    if (!message.IntendedRecipientNodeNames.Contains(this.HyperNodeName))
                    {
                        // This node was not an intended recipient, so ignore the message, but still forward it if possible.
                        response.NodeAction = HyperNodeActionType.Ignored;
                        response.NodeActionReason = HyperNodeActionReasonType.UnintendedRecipient;
                        activityTracker.TrackIgnored("Message not intended for agent.");
                    }
                    else
                    {
                        // This node accepts responsibility for processing this message
                        response.NodeAction = HyperNodeActionType.Accepted;
                        response.NodeActionReason = HyperNodeActionReasonType.IntendedRecipient;
                        activityTracker.Track("Attempting to process message...");

                        // Define the method in a safe way (i.e. with a try/catch around it)
                        var processMessageInternalSafe = new Action<ProcessMessageInternalParameter>(
                            args =>
                            {
                                try
                                {
                                    ProcessMessageInternal(args);
                                }
                                catch (Exception ex)
                                {
                                    args.Response.ProcessStatusFlags = MessageProcessStatusFlags.Failure;
                                    args.ActivityTracker.TrackException(ex);
                                }
                                finally
                                {
                                    args.ActivityTracker.TrackProcessed();
                                }
                            }
                        );

                        var processMessageInternalArgs = new ProcessMessageInternalParameter(message, response, activityTracker, _tokenSource.Token);
                        if (message.RunConcurrently)
                        {
                            childTasks.Add(
                                Task.Factory.StartNew(
                                    args => processMessageInternalSafe(args as ProcessMessageInternalParameter),
                                    processMessageInternalArgs,
                                    _tokenSource.Token
                                )
                            );
                        }
                        else
                        {
                            processMessageInternalSafe(processMessageInternalArgs);
                        }
                    }

                    childTasks.AddRange(
                        ForwardMessage(message, activityTracker)
                    );
                }

                // Now that we have all of our tasks doing stuff, we want to add a final continuation when everything is finished to dispose of our subscribers
                Task.WhenAll(
                    childTasks.ToArray()
                ).ContinueWith(
                    (t, param) => ChildThreadCleanup(param as ChildThreadCleanupParameter),
                    new ChildThreadCleanupParameter(longLivedSubscribers, activityTracker, response)
                );
            }
            catch (Exception ex)
            {
                response.ProcessStatusFlags = MessageProcessStatusFlags.Failure;
                activityTracker.TrackException(ex);
            }
            finally
            {
                // Dispose of our short lived subscribers
                shortLivedSubscribers.Dispose();
            }

            return response;
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        /// <summary>
        /// This method provides one last chance to dispose of any subscribers that may still exist due to child threads not
        /// terminating as expected. This is also where the memory cache is disposed.
        /// </summary>
        public void Dispose()
        {
            // Dispose of any leftover subscribers
            if (_backupSubscriberReference != null && _backupSubscriberReference.Any())
            {
                lock (_lock)
                {
                    foreach (var subscriber in _backupSubscriberReference.Where(s => s != null))
                    {
                        subscriber.Dispose();
                    }
                }
            }

            // Dispose of any of our activity monitors that implement IDisposable
            foreach (var disposableMonitor in _activityMonitors.OfType<IDisposable>())
            {
                disposableMonitor.Dispose();
            }

            // Check if our ITaskIdProvider needs to be disposed
            var disposableTaskIdProvider = this.TaskIdProvider as IDisposable;
            if (disposableTaskIdProvider != null)
                disposableTaskIdProvider.Dispose();

            // Dispose of our activity cache
            if (_activityCache != null)
                _activityCache.Dispose();

            // Dispose of our cancellation source
            if (_tokenSource != null)
                _tokenSource.Dispose();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Initializes an instance of the <see cref="HyperNodeService"/> class with the specified name.
        /// </summary>
        /// <param name="hyperNodeName">The name of the <see cref="HyperNodeService"/>.</param>
        private HyperNodeService(string hyperNodeName)
        {
            _hyperNodeName = hyperNodeName;
            this.ActivityCacheSlidingExpiration = _defaultActivityCacheSlidingExpiration;
        }

        private IEnumerable<Task> ForwardMessage(HyperNodeMessageRequest message, HyperNodeServiceActivityTracker activityTracker)
        {
            Task[] forwardingTasks = { };

            var children = GetConnectedHyperNodeChildren(
                message.ForwardingPath.GetChildren(this.HyperNodeName).ToList(),
                activityTracker
            );
            if (children != null)
            {
                forwardingTasks = children.Select(
                    child => new ForwardingTaskParameter(child.Key, activityTracker, message)
                ).Select(
                    childForwardingParam => Task.Factory.StartNew(
                        param =>
                        {
                            var forwardingParam = (ForwardingTaskParameter)param;
                            HyperNodeMessageResponse response = null;

                            try
                            {
                                // By the time we get here, we must have guaranteed that a client endpoint exists with the specified name.
                                forwardingParam.ActivityTracker.TrackForwarding(forwardingParam.HyperNodeName);
                                response = new HyperNodeClient(
                                    forwardingParam.HyperNodeName
                                ).ProcessMessage(
                                    forwardingParam.Message
                                );
                            }
                            catch (FaultException)
                            {
                                // Rethrow fault exceptions to be handled by the continuation
                                throw;
                            }
                            catch (Exception ex)
                            {
                                forwardingParam.ActivityTracker.TrackException(ex);
                            }

                            return response;
                        },
                        childForwardingParam,
                        _tokenSource.Token
                    ).ContinueWith(
                        (forwardingTask, param) =>
                        {
                            var forwardingParam = (ForwardingTaskParameter)param;

                            // Check the status of the antecedent and report accordingly
                            switch (forwardingTask.Status)
                            {
                                case TaskStatus.RanToCompletion:
                                    if (forwardingTask.Result != null)
                                    {
                                        forwardingParam.ActivityTracker.TrackHyperNodeResponded(forwardingParam.HyperNodeName, forwardingTask.Result);
                                    }
                                    break;
                                case TaskStatus.Faulted:
                                    forwardingParam.ActivityTracker.Track(
                                        string.Format("An unhandled exception was thrown by HyperNode '{0}' as a fault.", forwardingParam.HyperNodeName),
                                        "This is normally caused by an error thrown in the ProcessMessage() method, but could also be caused by an extension or plugin. Exception detail follows:\r\n" +
                                        forwardingTask.Exception
                                    );
                                    break;
                                default:
                                    forwardingParam.ActivityTracker.TrackFormat(
                                        "Child task to forward message to HyperNode '{0}' completed with status '{1}'.",
                                        forwardingParam.HyperNodeName,
                                        forwardingTask.Status.ToString()
                                    );
                                    break;
                            }
                        },
                        childForwardingParam,
                        _tokenSource.Token
                    )
                ).ToArray();

                // If we're not running concurrently, then we are running synchronously, which means we need to wait for all the child nodes to return
                if (!message.RunConcurrently)
                {
                    try
                    {
                        // We'll only wait so long. We let the client tell us how long they are willing to wait for the child nodes, but if this timeout
                        // is longer than the WCF timeout specified in the app.config, then the app.config will win by default. We'll also quit if
                        // cancellation is requested
                        Task.WaitAll(
                            forwardingTasks,
                            (int)Math.Min(message.ForwardingTimeout.TotalMilliseconds, int.MaxValue), // Make sure we don't accidentally cast more milliseconds than can fit in an instance of System.Int32
                            _tokenSource.Token
                        );
                    }
                    catch (Exception ex)
                    {
                        activityTracker.TrackException(ex);
                    }

                    // Track any HyperNodes that didn't return before the forwarding timeout
                    foreach (var forwardingArgs in forwardingTasks.Where(t => !t.IsCompleted).Select(t => t.AsyncState as ForwardingTaskParameter))
                    {
                        activityTracker.Track(
                            string.Format("HyperNode '{0}' timed out at {1}.",
                                forwardingArgs.HyperNodeName,
                                message.ForwardingTimeout
                            ),
                            string.Format(
                                "HyperNode '{0}' did not return a response before the specified timeout of {1}. This is usually caused by a long-running task in the " +
                                "recipient node or one of its descendants. Consider increasing the value of the ForwardingTimeout property in the message or lengthening " +
                                "the message timeout attributes in your WCF configuration. If caching was enabled for this message, additional trace logs may be obtained " +
                                "by querying the intended recipients directly using the message guid '{2}'. Additional trace logs may also be obtained by querying the " +
                                "data stores for any custom activity monitors that may be attached to the intended recipients.",
                                forwardingArgs.HyperNodeName,
                                message.ForwardingTimeout,
                                message.MessageGuid
                            )
                        );
                    }
                }
            }

            return forwardingTasks;
        }

        private IEnumerable<HyperNodeVertex> GetConnectedHyperNodeChildren(List<HyperNodeVertex> vertices, HyperNodeServiceActivityTracker activityTracker)
        {
            // Check to see if the path specified any child nodes that don't have endpoints defined in the app.config
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModelGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
            if (serviceModelGroup == null)
            {
                // No endpoints exist because no service model section exists
                activityTracker.TrackFormat(
                    "Configuration does not contain a serviceModel section. Message forwarding is disabled.",
                    this.HyperNodeName
                );

                // Just clear the list so we don't try to forward to anyone
                vertices.Clear();
            }
            else
            {
                // Remove all vertices that don't have endpoint configurations in the app.config
                vertices.RemoveAll(
                    v =>
                    {
                        var endpointExists = serviceModelGroup.Client.Endpoints
                            .Cast<ChannelEndpointElement>()
                            .Any(e => e.Name == v.Key && e.Contract == typeof(IHyperNodeService).FullName
                        );

                        if (!endpointExists)
                            activityTracker.TrackFormat("Message could not be forwarded to HyperNode '{0}' because no client endpoint was found with that name.", v.Key);

                        return !endpointExists;
                    }
                );
            }
            return vertices;
        }

        private void ProcessMessageInternal(ProcessMessageInternalParameter args)
        {
            ICommandResponse commandResponse;

            CommandModuleConfiguration commandModuleConfig;
            if (_commandModuleConfigurations.ContainsKey(args.Message.CommandName) &&
                _commandModuleConfigurations.TryGetValue(args.Message.CommandName, out commandModuleConfig) &&
                commandModuleConfig.Enabled)
            {
                // Create our command module instance
                var commandModule = (ICommandModule)Activator.CreateInstance(commandModuleConfig.CommandModuleType);

                ICommandRequestSerializer requestSerializer = null;
                ICommandResponseSerializer responseSerializer = null;

                // Check if our command module is able to create request and/or response serializers
                var requestSerializerFactory = commandModule as ICommandRequestSerializerFactory;
                var responseSerializerFactory = commandModule as ICommandResponseSerializerFactory;

                // Use the factories to create serializers, if applicable
                if (requestSerializerFactory != null)
                    requestSerializer = requestSerializerFactory.Create();
                if (responseSerializerFactory != null)
                    responseSerializer = responseSerializerFactory.Create();

                // Allow the command module factory-created serializers to take precedence over the configured serializers
                requestSerializer = requestSerializer ?? commandModuleConfig.RequestSerializer ?? DefaultRequestSerializer;
                responseSerializer = responseSerializer ?? commandModuleConfig.ResponseSerializer ?? DefaultResponseSerializer;

                try
                {
                    // Deserialize the request string
                    var commandRequest = requestSerializer.Deserialize(args.Message.CommandRequestString);

                    // Create the execution context to pass into our module
                    var context = new CommandExecutionContext
                    {
                        TaskId = args.Response.TaskId,
                        MessageGuid = args.Message.MessageGuid,
                        CommandName = args.Message.CommandName,
                        CreatedByAgentName = args.Message.CreatedByAgentName,
                        CreationDateTime = args.Message.CreationDateTime,
                        Request = commandRequest,
                        Activity = args.ActivityTracker,
                        Token = args.Token
                    };

                    // Copy in the info from our top-level message and response. We're using AddRange() for collections instead
                    // of just assignment so that if the user changes anything, it doesn't affect the top-level message
                    context.IntendedRecipientNodeNames.AddRange(args.Message.IntendedRecipientNodeNames);
                    context.SeenByNodeNames.AddRange(args.Message.SeenByNodeNames);

                    // Execute the command
                    // TODO: Process the message
                    // TODO: Make calls to activityTracker.Track() as needed
                    // TODO: If non-fatal errors are encountered during processing, set the HadNonFatalErrors flag: response.ProcessStatusFlags |= MessageProcessStatusFlags.HadNonFatalErrors;
                    // TODO: If warnings are encountered during processing, set the HadWarnings flag: response.ProcessStatusFlags |= MessageProcessStatusFlags.HadWarnings;
                    // TODO: If we successfully process the message, set response.ProcessStatusFlags equal to Success
                    commandResponse = commandModule.Execute(context);

                    // Serialize the response to send back
                    args.Response.CommandResponseString = responseSerializer.Serialize(commandResponse);
                }
                finally
                {
                    // Check if our module is disposable and take care of it appropriately
                    var disposableCommandModule = commandModule as IDisposable;
                    if (disposableCommandModule != null)
                        disposableCommandModule.Dispose();
                }
            }
            else
            {
                // Unrecognized command
                commandResponse = new CommandResponse(MessageProcessStatusFlags.Failure | MessageProcessStatusFlags.InvalidCommand);
                args.ActivityTracker.TrackFormat("Fatal error: Invalid command '{0}'.", args.Message.CommandName);
            }

            args.Response.ProcessStatusFlags = commandResponse.ProcessStatusFlags;
        }

        private void ChildThreadCleanup(ChildThreadCleanupParameter args)
        {
            /* Signal completion before we dispose our subscribers. This is necessary because clients who are polling the service for progress
             * updates must know when the service is done sending updates. Make sure we pass the final, completed response object in case we have
             * any monitors that are watching for it. */
            args.ActivityTracker.TrackFinished(args.Response);

            // We're about to dispose of these subscribers, so we no longer need the backup reference
            if (_backupSubscriberReference != null)
            {
                lock (_lock)
                {
                    _backupSubscriberReference.Remove(args.ToDispose);
                }
            }

            // Finally, dispose the subscribers and we're home free!
            if (args.ToDispose != null)
                args.ToDispose.Dispose();
        }

        #region Static

        private static HyperNodeService Create()
        {
            var config = (HyperNodeConfigurationSection)ConfigurationManager.GetSection("hyperNet/hyperNode");

            var service = new HyperNodeService(config.HyperNodeName)
            {
                EnableActivityCache = config.EnableActivityCache,
                ActivityCacheSlidingExpiration = TimeSpan.FromMinutes(config.ActivityCacheSlidingExpirationMinutes)
            };

            ConfigureSystemCommands(service, config);
            ConfigureTaskProvider(service, config);
            ConfigureActivityMonitors(service, config);
            ConfigureCommandModules(service, config);

            return service;
        }

        private static void ConfigureSystemCommands(HyperNodeService service, HyperNodeConfigurationSection config)
        {
            // TODO: Bring the config into this somehow. If nothing else, need to be able to enable/disable system commands via config.
            if (!service._commandModuleConfigurations.TryAdd(
                    "GetCachedTaskProgressInfo",
                    new CommandModuleConfiguration
                    {
                        CommandName = "GetCachedTaskProgressInfo",
                        Enabled = true, // TODO: Should be set from config
                        CommandModuleType = typeof(GetCachedTaskProgressInfoCommand),
                        RequestSerializer = new NetDataContractRequestSerializer<GetCachedTaskProgressInfoRequest>(),
                        ResponseSerializer = new NetDataContractResponseSerializer<HyperNodeTaskProgressInfo>()
                    }
                 )
                )
            {
                throw new DuplicateCommandException("A command already exists with the name 'GetCachedProgressInfo'.");
            }
            if (!service._commandModuleConfigurations.TryAdd(
                    "ValidCommand",
                    new CommandModuleConfiguration
                    {
                        CommandName = "ValidCommand",
                        Enabled = true, // TODO: Should be set from config
                        CommandModuleType = typeof(ValidCommandTest)
                    }
                 )
                )
            {
                throw new DuplicateCommandException("A command already exists with the name 'ValidCommand'.");
            }
            if (!service._commandModuleConfigurations.TryAdd(
                    "LongRunningTaskTest",
                    new CommandModuleConfiguration
                    {
                        CommandName = "LongRunningTaskTest",
                        Enabled = true, // TODO: Should be set from config
                        CommandModuleType = typeof(LongRunningCommandTest)
                    }
                 )
                )
            {
                throw new DuplicateCommandException("A command already exists with the name 'LongRunningTaskTest'.");
            }
            if (!service._commandModuleConfigurations.TryAdd(
                    "SuperLongRunningTestTask",
                    new CommandModuleConfiguration
                    {
                        CommandName = "SuperLongRunningTestTask",
                        Enabled = true, // TODO: Should be set from config
                        CommandModuleType = typeof(SuperLongRunningCommandTest)
                    }
                 )
                )
            {
                throw new DuplicateCommandException("A command already exists with the name 'SuperLongRunningTestTask'.");
            }
        }

        private static void ConfigureTaskProvider(HyperNodeService service, HyperNodeConfigurationSection config)
        {
            ITaskIdProvider taskIdProvider = null;

            // Set our task id provider if applicable, but if we have any problems creating the instance or casting to ITaskIdProvider, we deliberately want to fail out and make them fix the app.config
            if (!string.IsNullOrWhiteSpace(config.TaskIdProviderType))
            {
                taskIdProvider = (ITaskIdProvider)Activator.CreateInstance(Type.GetType(config.TaskIdProviderType, true));
                taskIdProvider.Initialize();
            }

            service.TaskIdProvider = taskIdProvider ?? DefaultTaskIdProvider;
        }

        private static void ConfigureActivityMonitors(HyperNodeService service, HyperNodeConfigurationSection config)
        {
            // Instantiate our activity monitors
            foreach (var monitorConfig in config.ActivityMonitors)
            {
                // If we have any problems creating the instance or casting to HyperNodeServiceActivityMonitor, we deliberately want to fail out and make them fix the app.config
                var monitor = (HyperNodeServiceActivityMonitor)Activator.CreateInstance(Type.GetType(monitorConfig.Type, true));
                if (monitor != null)
                {
                    monitor.Name = monitorConfig.Name;
                    monitor.Enabled = monitorConfig.Enabled;

                    monitor.Initialize();

                    lock (service._lock)
                    {
                        if (service._activityMonitors.Any(m => m.Name == monitorConfig.Name))
                        {
                            throw new DuplicateActivityMonitorException(
                                string.Format("An activity monitor already exists with the name '{0}'.", monitorConfig.Name)
                            );
                        }

                        service._activityMonitors.Add(monitor);
                    }
                }
            }
        }

        private static void ConfigureCommandModules(HyperNodeService service, HyperNodeConfigurationSection config)
        {
            Type defaultRequestSerializerType = null;
            Type defaultResponseSerializerType = null;

            // First, see if we have any serializer types defined at the collection level
            if (!string.IsNullOrWhiteSpace(config.CommandModules.RequestSerializerType))
                defaultRequestSerializerType = Type.GetType(config.CommandModules.RequestSerializerType, true);
            if (!string.IsNullOrWhiteSpace(config.CommandModules.ResponseSerializerType))
                defaultResponseSerializerType = Type.GetType(config.CommandModules.ResponseSerializerType, true);

            foreach (var commandModuleConfig in config.CommandModules)
            {
                var commandModuleType = Type.GetType(commandModuleConfig.Type, true);
                if (commandModuleType.GetInterfaces().Contains(typeof(ICommandModule)))
                {
                    Type commandRequestSerializerType = null;
                    Type commandResponseSerializerType = null;

                    // Now check to see if we have any serializer types defined at the command level
                    if (!string.IsNullOrWhiteSpace(commandModuleConfig.RequestSerializerType))
                        commandRequestSerializerType = Type.GetType(commandModuleConfig.RequestSerializerType, true);
                    if (!string.IsNullOrWhiteSpace(commandModuleConfig.ResponseSerializerType))
                        commandResponseSerializerType = Type.GetType(commandModuleConfig.ResponseSerializerType, true);

                    // Our final configuration allows command-level serializer types to take precedence, if available. Otherwise, the collection-level types are used.
                    var configRequestSerializerType = commandRequestSerializerType ?? defaultRequestSerializerType;
                    var configResponseSerializerType = commandResponseSerializerType ?? defaultResponseSerializerType;

                    ICommandRequestSerializer configRequestSerializer = null;
                    ICommandResponseSerializer configResponseSerializer = null;

                    // Attempt construction of config-level serializer types
                    if (configRequestSerializerType != null)
                        configRequestSerializer = (ICommandRequestSerializer)Activator.CreateInstance(configRequestSerializerType);
                    if (configResponseSerializerType != null)
                        configResponseSerializer = (ICommandResponseSerializer)Activator.CreateInstance(configResponseSerializerType);

                    // Finally, construct our command module configuration
                    var commandConfig = new CommandModuleConfiguration
                    {
                        CommandName = commandModuleConfig.Name,
                        Enabled = commandModuleConfig.Enabled,
                        CommandModuleType = commandModuleType,
                        RequestSerializer = configRequestSerializer ?? DefaultRequestSerializer,
                        ResponseSerializer = configResponseSerializer ?? DefaultResponseSerializer
                    };

                    // If this fails, a command with the specified name already exists
                    if (!service._commandModuleConfigurations.TryAdd(commandModuleConfig.Name, commandConfig))
                    {
                        throw new DuplicateCommandException(
                            string.Format("A command already exists with the name '{0}'.", commandModuleConfig.Name)
                        );
                    }
                }
            }
        }

        #endregion Static

        #endregion Private Methods

        #region Internal Helper Methods

        internal HyperNodeTaskProgressInfo GetCachedTaskProgressInfo(Guid messageGuid, string taskId)
        {
            return _activityCache.GetTaskProgressInfo(messageGuid, taskId);
        }

        // TODO: Write helper for "Discover" command (no params, searches config and forwards command to all children.)
        // TODO: Write helper for "GetSettings" command (which settings, in particular?)
        // TODO: Write helper for "EnableCommand" command (input name of command to enable)
        // TODO: Write helper for "DisableCommand" command (input name of command to disable)
        // TODO: Write helper for "EnableActivityMonitor" command (input name of monitor to enable)
        // TODO: Write helper for "DisableActivityMonitor" command (input name of monitor to disable)
        // TODO: Write helper for "RenameActivityMonitor" command (input old name and new name of monitor to rename)

        /*************************************************************************************************************************************
         * Cancellation Notes
         * 
         * I'm considering creating new cancellation token sources as requests come in. There will be the top-level cancellation triggered
         * by a call to the HyperNode's Cancel() method, but there could also be task-level and message-level cancellation token sources.
         * 
         * A task-level cancellation token source should trigger the cancellation of a single task, but any other tasks currently being
         * processed should be left alone. Other HyperNodes processing the same message that spawned the cancelled task for one HyperNode
         * should not be affected. In other words, if I ask Alice and Bob to process the same message, but then I send a task-level
         * cancellation to Alice, Alice should cancel her task, but Bob should continue processing the message.
         * 
         * A message-level cancellation token source should trigger cancellation of all tasks spawned for that message GUID across all
         * HyperNodes in the network. In this case, if I ask Alice and Bob to process the same message, and then I send a message-level
         * cancellation to Alice, Alice should cancel her task and forward the cancellation request to all of her children, including Bob, who
         * will do likewise. Bob will also cancel his task, and so will every other HyperNode in the network.
         *************************************************************************************************************************************/

        // TODO: Write helper for "CancelMessage" command (input message GUID of message to cancel, forwards command to all children. Intended to cancel an entire message, which could have gone to any number of nodes.)
        // TODO: Write helper for "CancelTask" command (input task id of task to cancel, DOES NOT forward command to children. Intended to target cancellation for a single task for a single node.)

        #endregion Internal Helper Methods
    }
}
