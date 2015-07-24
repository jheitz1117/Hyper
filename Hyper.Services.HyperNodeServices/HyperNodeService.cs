using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Hyper.ActivityTracking;
using Hyper.Services.HyperNodeActivityTracking;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;
using Hyper.Services.HyperNodeProxies;
using Hyper.Services.HyperNodeServices.ActivityTracking;

namespace Hyper.Services.HyperNodeServices
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

        private readonly object _lock = new object();
        private readonly ITaskIdProvider _defaultTaskIdProvider = new GuidTaskIdProvider();
        private readonly TimeSpan _defaultActivityCacheSlidingExpiration = TimeSpan.FromHours(1);
        private readonly CachedProgressInfoCollector _activityCache = new CachedProgressInfoCollector();
        private readonly string _hyperNodeName;
        private readonly List<HyperNodeServiceActivityMonitor> _activityMonitors = new List<HyperNodeServiceActivityMonitor>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        /// This member is meant to store a backup of all of the <see cref="IDisposable" /> subscribers to our activity feed.
        /// If the <see cref="HyperNodeService"/> object is forced to be disposed while it still has child threads processing,
        /// the <see cref="ChildThreadCleanup" /> method may not be called properly. In this case, we would lose our only
        /// reference to the <see cref="IDisposable" /> subscribers waiting to be disposed, resulting in a memory leak. To
        /// mitigate this possibility, this backup reference is stored so that we have one last chance to clean up when the
        /// <see cref="HyperNodeService" /> object is disposed.
        /// </summary>
        private readonly List<IDisposable> _backupSubscriberReference = new List<IDisposable>();

        #endregion

        #region Properties

        private string HyperNodeName
        {
            get { return _hyperNodeName; }
        }

        private ITaskIdProvider _taskIdProvider;
        public ITaskIdProvider TaskIdProvider
        {
            private get { return _taskIdProvider ?? _defaultTaskIdProvider; }
            set { _taskIdProvider = value ?? _defaultTaskIdProvider; }
        }

        public bool EnableActivityCache { get; set; }

        public TimeSpan ActivityCacheSlidingExpiration
        {
            get { return _activityCache.CacheDuration; }
            set { _activityCache.CacheDuration = value; }
        }

        #endregion Properties

        /// <summary>
        /// Initializes an instance of the <see cref="HyperNodeService"/> class with the specified name.
        /// </summary>
        /// <param name="hyperNodeName">The name of the <see cref="HyperNodeService"/>.</param>
        public HyperNodeService(string hyperNodeName)
        {
            _hyperNodeName = hyperNodeName;
            this.ActivityCacheSlidingExpiration = _defaultActivityCacheSlidingExpiration;
        }

        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
        {
            var response = new HyperNodeMessageResponse(this.HyperNodeName)
            {
                NodeAction = HyperNodeActionType.None,
                NodeActionReason = HyperNodeActionReasonType.Unknown,
                ProcessStatusFlags = MessageProcessStatusFlags.None
            };

            var activityTracker = new HyperNodeServiceActivityTracker(this.HyperNodeName, message);

            #region Activity Tracker Setup

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
                    // Cast all our activity items as HyperNodeActivityEventItem
                    a => a.EventArgs.ActivityItem as HyperNodeActivityEventItem
                );

                /*****************************************************************************************************************
                 * Subscribe our progress cache to our event stream only if the client requested it and the feature is actually enabled. There is currently no built-in
                 * functionality to support long-running task tracing other than the memory cache. If the client opts to disable the memory cache to save resources,
                 * then they will need to setup a custom HyperNodeServiceActivityMonitor if they still want to be able to know what's going on in the server. Custom
                 * HyperNodeServiceActivityMonitor objects can record activity to a database or some other data store, which the user can then query for the desired
                 * activity.
                 *****************************************************************************************************************/
                if (this.EnableActivityCache && message.CacheProgressInfo)
                    longLivedSubscribers.Add(liveEvents.Subscribe(_activityCache));

                /*****************************************************************************************************************
                 * If we want a task trace returned, that's fine, but the task trace should only ever contain events recorded during the current call to ProcessMessage()
                 * as opposed to the entire lifetime of a task in general. This is because a task can be spawned in a child thread and can outlive the original call to
                 * ProcessMessage(). In this case, even if we did record events of the task, they would never be returned to the caller. This is the whole reason why we
                 * have a CachedProgressInfoCollector that is in charge of collecting trace information for tasks that run concurrently. It should be noted that if the
                 * client elects to use caching AND return a task trace, then the events recorded in the task trace and those recorded in the cache will likely overlap
                 * for events which fired before ProcessMessage() returned. However, any events that fired after ProcessMessage() returned would only be recorded in the
                 * cache. This may result in a task trace that looks incomplete since the "processing complete" message would not have occured before the method returned.
                 * 
                 * All that being said, we never want to add our task trace collector to the global list of subscribers. Instead, we will limit the scope of the task
                 * trace collector to ONLY the current call, and we will allow the cache and custom monitors to record any events that occur in child threads after the
                 * call to ProcessMessage() returns.
                 *****************************************************************************************************************/
                if (message.ReturnTaskTrace)
                {
                    // See description above for why this is a short-lived subscriber instead of a long-lived one
                    var taskTraceCollector = new HyperNodeTaskTraceCollector(response);
                    shortLivedSubscribers.Add(
                        liveEvents
                            .Where(a => taskTraceCollector.ShouldTrack(a))
                            .Subscribe(taskTraceCollector)
                    );
                }

                /*****************************************************************************************************************
                 * Concurrent tasks cannot return any meaningful responses because the main thread gets to the return statement long before the child threads complete
                 * their processing. Any response that is returned by the main thread contains minimal information: perhaps a cursory task trace and some enum values,
                 * but that's it. Therefore, by design, clients who elect to run tasks concurrently inherently decide to disregard the response objects in favor of
                 * a progress cache which can be queried after the initial call. This cache can either be the in-memory cache of the hypernode, or it can be some other
                 * repository that the user sets up themselves via a custom monitor.
                 * 
                 * On the other hand, if the client elects to run non-concurrently (aka synchronously), then it expects to and should receive a well-formed response
                 * object. For this, we have a response collector whose sole job it is to collect response objects for child nodes to which the message is forwarded
                 * and return an aggregate response to the client. The response has a tree-like structure in which child node responses are referenced by their node
                 * name. Because the request is synchronous, this monitor will not need to live beyond the scope of this call. Hence, we add it to the collection of
                 * short-lived subscribers.
                 *****************************************************************************************************************/
                if (!message.RunConcurrently)
                {
                    var responseCollector = new HyperNodeMessageResponseCollector(response);
                    shortLivedSubscribers.Add(
                        liveEvents
                            .Where(a => responseCollector.ShouldTrack(a))
                            .Subscribe(responseCollector)
                    );
                }

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
                                            new ActivityMonitorAttachmentException(
                                                string.Format(
                                                    "Unable to attach monitor '{0}' because its ShouldTrack() method threw an exception.",
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
                    new ActivityTrackerSetupException(
                        "An exception was thrown while setting up the activity tracker. See inner exception for details.",
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
                // Assign a task id to our result object using a custom provider.
                // As of 6:39 a.m. on 7/18/2015, nothing in the HyperNodeService depends on the TaskId, so if the provider returns an empty string it won't break anything.
                // The Task Id is just here for reference for the consuming services and clients. However, if the provider throws an exception (it is user code after all),
                // at least it won't crash the server since we're in a try/catch block.
                response.TaskId = this.TaskIdProvider.CreateTaskId(message);

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
                                    response.ProcessStatusFlags = MessageProcessStatusFlags.Failure;
                                    activityTracker.TrackException(ex);
                                }
                                finally
                                {
                                    activityTracker.TrackProcessed();
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
                    new ChildThreadCleanupParameter(longLivedSubscribers, activityTracker)
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

        public void AddActivityMonitor(HyperNodeServiceActivityMonitor monitor)
        {
            _activityMonitors.Add(monitor);
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

            // Dispose of our activity cache
            if (_activityCache != null)
                _activityCache.Dispose();

            // Dispose of our cancellation source
            if (_tokenSource != null)
                _tokenSource.Dispose();
        }

        #region Private Methods

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
            /*
             * The way I would like for this to work is to have various custom tasks defined in the app.config. Then when we switch on the message.CommandName,
             * the command name would be assigned to one of the tasks, which would then be initialized via reflection and setup properly with any parameters
             * passed in. Then we can just run the tasks's Execute() method (or whatever we want to call it).
             * 
             * One other idea is that we might could avoid having to switch on message.CommandName if we could simply dynamically find the matching task in the
             * config based on the CommandName passed in. This would save us from having to switch at all, and the best part is that the command names would not
             * be hard-coded! This is obviously the best way I've come up with so far!
             */

            /*
             * Crazy idea: should we delegate this message processing out to another custom behavior that can be specified in the app.config and written by
             * the users?
             */
            switch (args.Message.CommandName)
            {
                case "ValidCommand":
                    {
                        // This simulates a long-running task
                        //Thread.Sleep(10000);
                        args.ActivityTracker.Track("Got here past the sleep!");

                        //Thread.Sleep(3000);
                        args.ActivityTracker.Track("Another step in the process.");
                        //throw new Exception("Bad!");

                        // TODO: Process the message
                        // TODO: Make calls to activityTracker.Track() as needed
                        // TODO: If non-fatal errors are encountered during processing, set the HadNonFatalErrors flag: response.ProcessStatusFlags |= MessageProcessStatusFlags.HadNonFatalErrors;
                        // TODO: If warnings are encountered during processing, set the HadWarnings flag: response.ProcessStatusFlags |= MessageProcessStatusFlags.HadWarnings;

                        // TODO: If we successfully process the message, set response.ProcessStatusFlags equal to Success
                        args.Response.ProcessStatusFlags |= MessageProcessStatusFlags.Success | MessageProcessStatusFlags.HadNonFatalErrors | MessageProcessStatusFlags.HadWarnings;
                    } break;
                case "LongRunningTaskTest":
                    {
                        var progressTotal = 100;
                        args.ActivityTracker.TrackFormat("CommandName '{0}' recognized. Commencing execution of task.", args.Message.CommandName);
                        Thread.Sleep(3000);
                        args.ActivityTracker.Track("Progress update 1", 10, progressTotal);
                        Thread.Sleep(3000);
                        args.ActivityTracker.Track("Progress update 2", 20, progressTotal);
                        Thread.Sleep(3000);
                        args.ActivityTracker.Track("Progress update 3", 30, progressTotal);
                        Thread.Sleep(3000);
                        args.ActivityTracker.Track("Progress update 4", 44, progressTotal);
                        Thread.Sleep(3000);
                        args.ActivityTracker.Track("Progress update 5", 53, progressTotal);
                        Thread.Sleep(3000);
                        args.ActivityTracker.Track("Progress update 6", 72, progressTotal);
                        Thread.Sleep(6000);
                        args.ActivityTracker.Track("Progress update 7", 95, progressTotal);

                        args.ActivityTracker.Track("Progress update 8", progressTotal, progressTotal);
                        args.Response.ProcessStatusFlags |= MessageProcessStatusFlags.Success;
                    } break;
                case "SuperLongRunningTestTask":
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();

                        while (stopwatch.Elapsed <= TimeSpan.FromMinutes(1) && !args.Token.IsCancellationRequested)
                        {
                            args.ActivityTracker.TrackFormat("Elapsed time: {0}", stopwatch.Elapsed);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }

                        stopwatch.Stop();

                        args.ActivityTracker.TrackFormat("Out of loop at {0}. Cancellation was{1} requested.", stopwatch.Elapsed, (args.Token.IsCancellationRequested ? "" : " not"));
                        args.Response.ProcessStatusFlags |= MessageProcessStatusFlags.Cancelled;
                    }
                    break;
                case "GetCachedProgressInfo":
                    {
                        args.ActivityTracker.TrackFormat("Retrieving cached progress info for Message Guid '{0}'.", args.Message.MessageGuid);

                        // TODO: Fill out this custom request/response idea. So totally awesome!!
                        // Step 1: Deserialize request parameter XML into request object
                        // Step 2: Execute code based on request object to create a response object
                        // Step 3: Serialize the response object and return it in our HyperNodeResponse
                        // Should prolly use base class for this if possible, which can
                        var progressInfo = _activityCache.GetProgressInfo(args.Message.CommandRequestXml);
                        var builder = new StringBuilder();
                        using (var writer = new XmlTextWriter(new StringWriter(builder)))
                        {
                            progressInfo.WriteXml(writer);
                            writer.Flush();
                        }
                        args.Response.CommandResponseXml = builder.ToString();
                        args.Response.ProcessStatusFlags = MessageProcessStatusFlags.Success;
                    } break;
                default:
                    {
                        args.Response.ProcessStatusFlags = MessageProcessStatusFlags.Failure | MessageProcessStatusFlags.InvalidCommand;
                        args.ActivityTracker.TrackFormat("Fatal error: Invalid command '{0}'.", args.Message.CommandName);
                    } break;
            }
        }

        private void ChildThreadCleanup(ChildThreadCleanupParameter args)
        {
            /* Signal completion before we dispose our subscribers. This is necessary because clients who are polling the service for progress
             * updates must know when the service is done sending updates. */
            args.ActivityTracker.TrackFinished();

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

        #endregion Private Methods
    }
}
