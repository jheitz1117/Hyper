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
using Hyper.NodeServices.CommandModules;
using Hyper.NodeServices.Configuration;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Configuration;
using Hyper.NodeServices.TaskIdProviders;

// TODO: (Phase 2 - see comments below) ClearActivityCache 
/*************************************************************************************************************************************
 * ClearActivityCache
 * 
 * Everyone on StackOverflow seems to think this is impossible. Best option is to use the workaround posted by Thomas F. Abraham at
 * the following link: http://stackoverflow.com/questions/4183270/how-to-clear-the-net-4-memorycache/22388943#comment34789210_22388943
 * 
 * If we do clear the cache, then for cache items that are currently being accessed, is there any way to keep those and add an
 * activity item indicating that the cache was cleared? This might cut down on confusion later on when I'm watching a task and
 * suddenly lose all my progress. It would be nice to be informed that my progress disappeared because the cache was cleared.
 * 
 * Alternatively, I can implement my own ObjectCache (by implementing the .NET base class), or I can just roll my own
 * caching without using the .NET 4 base class. But not without an epic experiment to confirm it works. If we do this, would
 * probably need to go into its own DLL: Hyper.Caching.
 *************************************************************************************************************************************/

// TODO: (Phase 2 - see comments below) Self-documenting command modules and other user code
/*************************************************************************************************************************************
 * Documentation Notes (Phase 2)
 * 
 * Create a ICommandDocumenter interface that can be used to generate documentation for command modules.
 * 
 * Need to figure out what methods this should have. Should it just have a single void Document(StreamWriter writer) method? Or should
 * it have Write() methods for each individual piece of information?
 * 
 * Also, I originally thought of this specifically for custom command modules, but it makes sense to do it for other extensibility
 * points as well, especially activity monitors. If we just had a single Document() method, we could have a hierarchy of interfaces
 * to support various kinds of documentation, such as:
 * 
 * IDocumenter
 *    |
 *    void WriteDocument(StreamWriter writer)
 *    |
 *    +---ICommandDocumenter
 *    |       |
 *    |       void WriteCommandSummaryDocument(StreamWriter writer)
 *    |       void WriteCommandRequestDocument(StreamWriter writer)
 *    |       void WriteCommandResponseDocument(StreamWriter writer)
 *    |
 *    +---IActivityMonitorDocumenter
 *    |       |
 *    |       void WriteShouldTrackCriteriaDocument(StreamWriter writer)
 *    |       void WriteOnNextDocument(StreamWriter writer)
 *    |
 *    +---Etc.
 *************************************************************************************************************************************/

namespace Hyper.NodeServices
{
    /// <summary>
    /// Processes <see cref="HyperNodeMessageRequest"/> objects and returns <see cref="HyperNodeMessageResponse"/> objects.
    /// This class is a singleton and must be referenced using the static <see cref="HyperNodeService.Instance"/> property.
    /// This class cannot be inherited.
    /// </summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single
    )]
    public sealed partial class HyperNodeService : IHyperNodeService, IDisposable
    {
        #region Defaults

        private static readonly ITaskIdProvider DefaultTaskIdProvider = new GuidTaskIdProvider();
        private static readonly ICommandRequestSerializer DefaultRequestSerializer = new PassThroughSerializer();
        private static readonly ICommandResponseSerializer DefaultResponseSerializer = new PassThroughSerializer();
        private static readonly IHyperNodeConfigurationProvider DefaultConfigurationProvider = new HyperNodeSectionConfigurationProvider();

        #endregion Defaults

        #region Private Members

        private static readonly object Lock = new object();

        private readonly string _hyperNodeName;
        private readonly TaskProgressCacheMonitor _taskProgressCacheMonitor = new TaskProgressCacheMonitor();
        private readonly List<HyperNodeServiceActivityMonitor> _customActivityMonitors = new List<HyperNodeServiceActivityMonitor>();
        private readonly ConcurrentDictionary<string, CommandModuleConfiguration> _commandModuleConfigurations = new ConcurrentDictionary<string, CommandModuleConfiguration>();
        private readonly CancellationTokenSource _masterTokenSource = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, HyperNodeTaskInfo> _liveTasks = new ConcurrentDictionary<string, HyperNodeTaskInfo>();

        #endregion Private Members

        #region Properties

        private string HyperNodeName
        {
            get { return _hyperNodeName; }
        }

        private ITaskIdProvider TaskIdProvider { get; set; }

        internal bool EnableTaskProgressCache
        {
            get { return _taskProgressCacheMonitor.Enabled; }
            set { _taskProgressCacheMonitor.Enabled = value; }
        }

        internal bool EnableDiagnostics { get; set; }

        internal TimeSpan TaskProgressCacheDuration
        {
            get { return _taskProgressCacheMonitor.CacheDuration; }
            set { _taskProgressCacheMonitor.CacheDuration = value; }
        }

        internal int MaxConcurrentTasks { get; set; }

        /// <summary>
        /// Represents the singleton instance of the <see cref="HyperNodeService"/>.
        /// </summary>
        public static HyperNodeService Instance
        {
            get
            {
                if (_instance == null)
                    CreateAndConfigure(DefaultConfigurationProvider);

                return _instance;
            }
        } private static HyperNodeService _instance;

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Processes and/or forwards the specified message.
        /// </summary>
        /// <param name="message">The <see cref="HyperNodeMessageRequest"/> object to process.</param>
        /// <returns></returns>
        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
        {
            var response = new HyperNodeMessageResponse(this.HyperNodeName)
            {
                NodeAction = HyperNodeActionType.None,
                NodeActionReason = HyperNodeActionReasonType.Unknown,
                ProcessStatusFlags = MessageProcessStatusFlags.None
            };

            #region Create Task ID

            var taskId = "";
            HyperNodeActivityItem illBehavedTaskIdProviderActivityItem = null;
            var taskIdCreationContext = new TaskIdCreationContext(message.IntendedRecipientNodeNames, message.SeenByNodeNames)
            {
                CommandName = message.CommandName,
                CreatedByAgentName = message.CreatedByAgentName,
                CreationDateTime = message.CreationDateTime,
                ProcessOptionFlags = message.ProcessOptionFlags
            };

            try
            {
                // Try to use our custom task ID provider
                taskId = this.TaskIdProvider.CreateTaskId(taskIdCreationContext);

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
                taskId = DefaultTaskIdProvider.CreateTaskId(taskIdCreationContext);

                illBehavedTaskIdProviderActivityItem.EventDescription += " The default task ID provider was used to generate a non-blank task ID instead.";
                response.TaskTrace.Add(illBehavedTaskIdProviderActivityItem);
            }

            #endregion Create Task ID

            // Now that we have a valid task ID, try to add it to our dictionary
            var currentTaskInfo = new HyperNodeTaskInfo(_masterTokenSource.Token);

            // Check if we should reject this request for any reason
            var rejectionReason = GetRejectionReason(taskId, currentTaskInfo);
            if (rejectionReason.HasValue)
            {
                response.NodeAction = HyperNodeActionType.Rejected;
                response.NodeActionReason = rejectionReason.Value;
                response.ProcessStatusFlags = MessageProcessStatusFlags.Cancelled;
                response.TaskId = null;

                var rejectionActivityItem = new HyperNodeActivityItem(this.HyperNodeName);
                switch (rejectionReason.Value)
                {
                    case HyperNodeActionReasonType.MaxConcurrentTaskCountReached:
                        rejectionActivityItem.EventDescription = string.Format("The maximum number of concurrent tasks ({0}) has been reached.", this.MaxConcurrentTasks);
                        rejectionActivityItem.EventDetail = "An attempted was made to start a new task when the maximum number of concurrent tasks were already running. Consider increasing the value of MaxConcurrentTasks or setting it to -1 (unlimited).";
                        break;
                    case HyperNodeActionReasonType.CancellationRequested:
                        rejectionActivityItem.EventDescription = "The service is shutting down. No new tasks can be started.";
                        rejectionActivityItem.EventDetail = "The service-level cancellation token has been triggered. No new tasks are being spun up and all existing tasks are in the process of shutting down.";
                        break;
                    case HyperNodeActionReasonType.DuplicateTaskId:
                        rejectionActivityItem.EventDescription = string.Format("A task with ID '{0}' is already running.", taskId);
                        rejectionActivityItem.EventDetail = "A duplicate task ID was generated. This can occur when a new instance of singleton task is started while an existing instance of that task is running. If you know the task will complete, please try again after the task is finished. You may also consider cancelling the task and then restarting it.";
                        break;
                }

                response.TaskTrace.Add(rejectionActivityItem);
            }
            else
            {
                // In this case, we've confirmed that the new task was added, so start our task-level stopwatch and set our response task ID
                currentTaskInfo.StartStopwatch();
                response.TaskId = taskId;

                // Allows the task info to track the message and response
                currentTaskInfo.Message = message;
                currentTaskInfo.Response = response;

                // Initialize our activity tracker so we can track progress
                InitializeActivityTracker(currentTaskInfo);

                #region Process Message

                try
                {
                    // Confirm receipt of the message
                    currentTaskInfo.Activity.TrackReceived();

                    if (message.ExpirationDateTime <= DateTime.Now)
                    {
                        // Message has expired, so ignore it and do not forward it
                        response.NodeAction = HyperNodeActionType.Ignored;
                        response.NodeActionReason = HyperNodeActionReasonType.MessageExpired;
                        currentTaskInfo.Activity.TrackIgnored("Message expired on " + message.ExpirationDateTime.ToString("G") + ".");
                    }
                    else if (message.SeenByNodeNames.Contains(this.HyperNodeName))
                    {
                        // Message was already processed by this node, so ignore it and do not forward it (since it would have been forwarded the first time)
                        response.NodeAction = HyperNodeActionType.Ignored;
                        response.NodeActionReason = HyperNodeActionReasonType.PreviouslySeen;
                        currentTaskInfo.Activity.TrackIgnored("Message previously seen.");
                    }
                    else
                    {
                        // Add this node to the list of nodes who have seen this message
                        message.SeenByNodeNames.Add(this.HyperNodeName);
                        currentTaskInfo.Activity.TrackSeen();

                        // Check if this message has a list of intended recipients, and if this node was one of them.
                        // An empty recipients list means means the message is indended for all nodes in the forwarding path.
                        if (message.IntendedRecipientNodeNames.Any() && !message.IntendedRecipientNodeNames.Contains(this.HyperNodeName))
                        {
                            // This node was not an intended recipient, so ignore the message, but still forward it if possible.
                            response.NodeAction = HyperNodeActionType.Ignored;
                            response.NodeActionReason = HyperNodeActionReasonType.UnintendedRecipient;
                            currentTaskInfo.Activity.TrackIgnored("Message not intended for agent.");
                        }
                        else
                        {
                            // This node accepts responsibility for processing this message
                            response.NodeAction = HyperNodeActionType.Accepted;
                            response.NodeActionReason = HyperNodeActionReasonType.IntendedRecipient;
                            currentTaskInfo.Activity.Track("Attempting to process message...");

                            // Define the method in a safe way (i.e. with a try/catch around it)
                            var processMessageInternalSafe = new Action<HyperNodeTaskInfo>(
                                args =>
                                {
                                    try
                                    {
                                        ProcessMessageInternal(args);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is OperationCanceledException)
                                            args.Response.ProcessStatusFlags |= MessageProcessStatusFlags.Cancelled;
                                        else
                                            args.Response.ProcessStatusFlags = MessageProcessStatusFlags.Failure;

                                        if (ex is InvalidCommandRequestTypeException)
                                            args.Response.ProcessStatusFlags |= MessageProcessStatusFlags.InvalidCommandRequest;

                                        args.Activity.TrackException(ex);
                                    }
                                    finally
                                    {
                                        args.Activity.TrackProcessed();
                                    }
                                }
                            );

                            if (message.RunConcurrently)
                            {
                                currentTaskInfo.AddChildTask(
                                    Task.Factory.StartNew(
                                        args => processMessageInternalSafe(args as HyperNodeTaskInfo),
                                        currentTaskInfo,
                                        currentTaskInfo.Token
                                    )
                                );
                            }
                            else
                            {
                                processMessageInternalSafe(currentTaskInfo);
                            }
                        }

                        currentTaskInfo.AddChildTasks(
                            ForwardMessage(currentTaskInfo)
                        );
                    }

                    // Now that we have all of our tasks doing stuff, we need to make sure we clean up after ourselves.
                    if (message.RunConcurrently)
                    {
                        // If we're running concurrently, we want to return immediately and allow the clean up to occur as a continuation after the tasks are finished.
                        currentTaskInfo.WhenChildTasks().ContinueWith(t => TaskCleanUp(taskId));
                    }
                    else
                    {
                        // Otherwise, if we're running synchronously, we want to block until all our child threads are done, then clean up.
                        currentTaskInfo.WaitChildTasks(_masterTokenSource.Token);
                        TaskCleanUp(taskId);
                    }
                }
                catch (Exception ex)
                {
                    response.ProcessStatusFlags = MessageProcessStatusFlags.Failure;
                    currentTaskInfo.Activity.TrackException(ex);
                }

                #endregion Process Message
            }

            return response;
        }

        /// <summary>
        /// Creates the singleton instance of <see cref="HyperNodeService"/> using the specified <see cref="IHyperNodeConfigurationProvider"/>.
        /// </summary>
        /// <param name="configProvider">The <see cref="IHyperNodeConfigurationProvider"/> to use to configure the service.</param>
        public static void CreateAndConfigure(IHyperNodeConfigurationProvider configProvider)
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    if (_instance == null)
                        _instance = Create(configProvider);
                }
            }
        }

        /// <summary>
        /// Adds the specified <see cref="Type"/> as an enabled command module with the specified command name. Command modules
        /// added using this method do not have <see cref="ICommandRequestSerializer"/> or <see cref="ICommandResponseSerializer"/>
        /// imlementations defined.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandModuleType">The <see cref="Type"/> of the command module.</param>
        public void AddCommandModuleConfiguration(string commandName, Type commandModuleType)
        {
            AddCommandModuleConfiguration(commandName, commandModuleType, true, null, null);
        }

        /// <summary>
        /// Adds the specified <see cref="Type"/> as a command module with the specified command name and configuration options.
        /// </summary>
        /// <param name="commandName">The name of the command.</param>
        /// <param name="commandModuleType">The <see cref="Type"/> of the command module.</param>
        /// <param name="enabled">Indicates whether the command will be enabled immediately.</param>
        /// <param name="requestSerializer">The <see cref="ICommandRequestSerializer"/> implementation to use to serialize and deserialize request objects. This parameter can be null.</param>
        /// <param name="responseSerializer">The <see cref="ICommandResponseSerializer"/> implementation to use to serialize and deserialize response objects. This parameter can be null.</param>
        public void AddCommandModuleConfiguration(string commandName, Type commandModuleType, bool enabled, ICommandRequestSerializer requestSerializer, ICommandResponseSerializer responseSerializer)
        {
            AddCommandModuleConfiguration(
                new CommandModuleConfiguration
                {
                    CommandName = commandName,
                    CommandModuleType = commandModuleType,
                    Enabled = enabled,
                    RequestSerializer = requestSerializer,
                    ResponseSerializer = responseSerializer
                }
            );
        }

        /// <summary>
        /// Initiates a cancellation request.
        /// </summary>
        public void Cancel()
        {
            if (!_masterTokenSource.IsCancellationRequested)
                _masterTokenSource.Cancel();
        }

        /// <summary>
        /// Waits for all of the child <see cref="Task"/> objects to complete execution.
        /// </summary>
        public void WaitAllChildTasks()
        {
            Task.WaitAll(GetChildTasks().ToArray());
        }

        /// <summary>
        /// Waits for all of the child <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the tasks to complete.</param>
        public void WaitAllChildTasks(CancellationToken token)
        {
            Task.WaitAll(GetChildTasks().ToArray(), token);
        }

        /// <summary>
        /// Waits for all of the child <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely.</param>
        public void WaitAllChildTasks(TimeSpan timeout)
        {
            Task.WaitAll(GetChildTasks().ToArray(), timeout);
        }

        /// <summary>
        /// Waits for all of the child <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        public void WaitAllChildTasks(int millisecondsTimeout)
        {
            Task.WaitAll(GetChildTasks().ToArray(), millisecondsTimeout);
        }

        /// <summary>
        /// Waits for all of the child <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the tasks to complete.</param>
        public void WaitAllChildTasks(int millisecondsTimeout, CancellationToken token)
        {
            Task.WaitAll(GetChildTasks().ToArray(), millisecondsTimeout, token);
        }

        /// <summary>
        /// Releases disposable resources consumed by this <see cref="HyperNodeService"/> instance.
        /// </summary>
        public void Dispose()
        {
            // Dispose of our task info objects
            foreach (var taskInfo in _liveTasks.Values.Where(ti => ti != null))
            {
                taskInfo.Dispose();
            }

            // Dispose of any of our activity monitors that implement IDisposable
            foreach (var disposableMonitor in _customActivityMonitors.OfType<IDisposable>())
            {
                disposableMonitor.Dispose();
            }

            // Check if our ITaskIdProvider needs to be disposed
            var disposableTaskIdProvider = this.TaskIdProvider as IDisposable;
            if (disposableTaskIdProvider != null)
                disposableTaskIdProvider.Dispose();

            // Dispose of our task progress cache monitor
            if (_taskProgressCacheMonitor != null)
                _taskProgressCacheMonitor.Dispose();

            // Dispose of our master cancellation token source
            if (_masterTokenSource != null)
                _masterTokenSource.Dispose();
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
        }

        private void InitializeActivityTracker(HyperNodeTaskInfo currentTaskInfo)
        {
            currentTaskInfo.Activity = new HyperNodeServiceActivityTracker(
                new HyperNodeActivityContext(
                    this.HyperNodeName,
                    currentTaskInfo.Message.CommandName,
                    currentTaskInfo.Response.TaskId,
                    (currentTaskInfo.Message.ReturnTaskTrace || this.EnableDiagnostics)
                )
            );

            try
            {
                // Create our activity feed by bridging the event tracker with reactive extensions
                var liveEvents = Observable.FromEventPattern<TrackActivityEventHandler, TrackActivityEventArgs>(
                    h => currentTaskInfo.Activity.TrackActivityHandler += h,
                    h => currentTaskInfo.Activity.TrackActivityHandler -= h
                ).Select(
                    a => a.EventArgs.ActivityItem as IHyperNodeActivityEventItem // Cast all our activity items as IHyperNodeActivityEventItem
                );

                /*****************************************************************************************************************
                 * Subscribe our task progress cache monitor to our event stream only if the client requested it and the feature is actually enabled. There is currently no built-in
                 * functionality to support long-running task tracing other than the memory cache. If the client opts to disable the memory cache to save resources,
                 * then they will need to setup a custom HyperNodeServiceActivityMonitor if they still want to be able to know what's going on in the server. Custom
                 * HyperNodeServiceActivityMonitor objects can record activity to a database or some other data store, which the user can then query for the desired
                 * activity.
                 *****************************************************************************************************************/
                if (this.EnableTaskProgressCache && currentTaskInfo.Message.CacheTaskProgress)
                {
                    currentTaskInfo.ActivitySubscribers.Add(
                        liveEvents
                            .Where(a => _taskProgressCacheMonitor.ShouldTrack(a))
                            .Subscribe(_taskProgressCacheMonitor)
                    );
                }

                /*****************************************************************************************************************
                 * If we want a task trace returned, that's fine, but the task trace in the real-time response would only ever contain events recorded during the current
                 * call to ProcessMessage() as opposed to the entire lifetime of a task in general. This is because a task can be spawned in a child thread and can outlive
                 * the original call to ProcessMessage(). This is the whole reason why we have a TaskProgressCacheMonitor that is in charge of collecting trace information
                 * for tasks that run concurrently. It should be noted that if the client elects to use caching AND return a task trace, then the events recorded in the
                 * real-time task trace and those recorded in the cache will likely overlap for events which fired before ProcessMessage() returned. However, any events that
                 * fired after ProcessMessage() returned would only be recorded in the cache. This may result in a task trace that looks incomplete since the "processing
                 * complete" message would not have occured before the method returned.
                 *****************************************************************************************************************/
                if (currentTaskInfo.Message.ReturnTaskTrace)
                {
                    var responseTaskTraceMonitor = new ResponseTaskTraceMonitor(currentTaskInfo.Response);
                    currentTaskInfo.ActivitySubscribers.Add(
                        liveEvents
                            .Where(a => responseTaskTraceMonitor.ShouldTrack(a))
                            .Subscribe(responseTaskTraceMonitor)
                    );
                }

                /*****************************************************************************************************************
                 * Concurrent tasks cannot return anything meaningful in the real-time response because the main thread gets to the return statement long before the
                 * child threads complete their processing. Any response that is returned by the main thread contains minimal information: perhaps an incomplete task
                 * trace and some enum values, but that's it. Therefore, by design, clients who elect to run tasks concurrently inherently decide to disregard the
                 * real-time response object except for the task ID, which it must use to query a progress cache after the initial call. This cache can either be the
                 * in-memory cache of the hypernode, or it can be some other repository that the user sets up themselves via a custom monitor.
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
                var childNodeResponseMonitor = new ChildNodeResponseMonitor(currentTaskInfo.Response);
                currentTaskInfo.ActivitySubscribers.Add(
                    liveEvents
                        .Where(a => childNodeResponseMonitor.ShouldTrack(a))
                        .Subscribe(childNodeResponseMonitor)
                );

                /*****************************************************************************************************************
                 * Subscribe our custom activity monitors to the event stream last, just in case any of them throw exceptions. If they do, we've already setup our
                 * task trace and cache monitors at this point, so we can actually use the activity tracker to track the exceptions. This way, we can make sure that
                 * any exceptions thrown by user code are available to be reported to the client
                 *****************************************************************************************************************/
                currentTaskInfo.ActivitySubscribers.Add(
                    new CompositeDisposable(
                        from monitor in _customActivityMonitors
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
                                        currentTaskInfo.Activity.TrackException(
                                            new ActivityMonitorSubscriptionException(
                                                string.Format(
                                                    "Unable to subscribe activity monitor '{0}' because its ShouldTrack() method threw an exception.",
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
                currentTaskInfo.Activity.TrackException(
                    new ActivityTrackerInitializationException(
                        "An exception was thrown while initializing the activity tracker. See inner exception for details.",
                        ex
                    )
                );
            }
        }

        private HyperNodeActionReasonType? GetRejectionReason(string taskId, HyperNodeTaskInfo taskInfo)
        {
            HyperNodeActionReasonType? rejectionReason = null;

            // Check the reason why we are rejecting
            if (this.MaxConcurrentTasks > -1 && _liveTasks.Count >= this.MaxConcurrentTasks)
                rejectionReason = HyperNodeActionReasonType.MaxConcurrentTaskCountReached;
            else if (_masterTokenSource.IsCancellationRequested)
                rejectionReason = HyperNodeActionReasonType.CancellationRequested;
            else if (!_liveTasks.TryAdd(taskId, taskInfo))
                rejectionReason = HyperNodeActionReasonType.DuplicateTaskId;

            return rejectionReason;
        }

        /// <summary>
        /// Forwards the specified <see cref="HyperNodeMessageRequest"/> object using the specified <see cref="HyperNodeTaskInfo"/> object.
        /// </summary>
        /// <param name="taskInfo">The <see cref="HyperNodeTaskInfo"/> object containing the information to forward.</param>
        /// <returns></returns>
        private IEnumerable<Task> ForwardMessage(HyperNodeTaskInfo taskInfo)
        {
            Task[] forwardingTasks = { };

            // Allow a null path to be passed. We can just treat it as an empty path and not forward the message to anyone
            var children = GetConnectedHyperNodeChildren(
                (taskInfo.Message.ForwardingPath ?? new HyperNodePath()).GetChildren(this.HyperNodeName).ToList(),
                taskInfo.Activity
            );

            if (children != null)
            {
                forwardingTasks = children.Select(
                    child => new ForwardingTaskParameter(child.Key, taskInfo)
                ).Select(
                    childForwardingParam => Task.Factory.StartNew(
                        param =>
                        {
                            var forwardingParam = (ForwardingTaskParameter)param;
                            HyperNodeMessageResponse response = null;

                            try
                            {
                                // By the time we get here, we must have guaranteed that a client endpoint exists with the specified name.
                                forwardingParam.TaskInfo.Activity.TrackForwarding(forwardingParam.ChildNodeName);
                                response = new HyperNodeClient(
                                    forwardingParam.ChildNodeName
                                ).ProcessMessage(
                                    forwardingParam.TaskInfo.Message
                                );
                            }
                            catch (FaultException)
                            {
                                // Rethrow fault exceptions to be handled by the continuation
                                throw;
                            }
                            catch (Exception ex)
                            {
                                forwardingParam.TaskInfo.Activity.TrackException(ex);
                            }

                            return response;
                        },
                        childForwardingParam,
                        taskInfo.Token
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
                                        forwardingParam.TaskInfo.Activity.TrackHyperNodeResponded(forwardingParam.ChildNodeName, forwardingTask.Result);
                                    }
                                    break;
                                case TaskStatus.Faulted:
                                    forwardingParam.TaskInfo.Activity.Track(
                                        string.Format("An unhandled exception was thrown by HyperNode '{0}' as a fault.", forwardingParam.ChildNodeName),
                                        "This is normally caused by an error thrown in the ProcessMessage() method, but could also be caused by an extension or plugin. Exception detail follows:\r\n" +
                                        forwardingTask.Exception
                                    );
                                    break;
                                default:
                                    forwardingParam.TaskInfo.Activity.TrackFormat(
                                        "Child task to forward message to HyperNode '{0}' completed with status '{1}'.",
                                        forwardingParam.ChildNodeName,
                                        forwardingTask.Status.ToString()
                                    );
                                    break;
                            }
                        },
                        childForwardingParam,
                        taskInfo.Token
                    )
                ).ToArray();

                // If we're not running concurrently, then we are running synchronously, which means we need to wait for all the child nodes to return
                if (!taskInfo.Message.RunConcurrently)
                {
                    try
                    {
                        // We'll only wait so long. We let the client tell us how long they are willing to wait for the child nodes, but if this timeout
                        // is longer than the WCF timeout specified in the app.config, then the app.config will win by default. We'll also quit if
                        // cancellation is requested
                        Task.WaitAll(
                            forwardingTasks,
                            (int)Math.Min(taskInfo.Message.ForwardingTimeout.TotalMilliseconds, int.MaxValue), // Make sure we don't accidentally cast more milliseconds than can fit in an instance of System.Int32
                            taskInfo.Token
                        );
                    }
                    catch (Exception ex)
                    {
                        taskInfo.Activity.TrackException(ex);
                    }

                    // Track any HyperNodes that didn't return before the forwarding timeout
                    foreach (var forwardingArgs in forwardingTasks.Where(t => !t.IsCompleted).Select(t => t.AsyncState as ForwardingTaskParameter))
                    {
                        taskInfo.Activity.Track(
                            string.Format("HyperNode '{0}' timed out at {1}.",
                                forwardingArgs.ChildNodeName,
                                taskInfo.Message.ForwardingTimeout
                            ),
                            string.Format(
                                "HyperNode '{0}' did not return a response before the specified timeout of {1}. This is usually caused by a long-running task in the " +
                                "recipient node or one of its descendants. Consider increasing the value of the ForwardingTimeout property in the message or lengthening " +
                                "the message timeout attributes in your WCF configuration. If caching was enabled for this message, additional trace logs may be obtained " +
                                "by querying the intended recipients directly. Additional trace logs may also be obtained by querying the data stores for any custom " +
                                "activity monitors that may be attached to the intended recipients.",
                                forwardingArgs.ChildNodeName,
                                taskInfo.Message.ForwardingTimeout
                            )
                        );
                    }
                }
            }

            return forwardingTasks;
        }

        private static IEnumerable<HyperNodeVertex> GetConnectedHyperNodeChildren(List<HyperNodeVertex> vertices, ITaskActivityTracker activityTracker)
        {
            // Check to see if the path specified any child nodes that don't have endpoints defined in the app.config
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModelGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
            if (serviceModelGroup == null)
            {
                // No endpoints exist because no service model section exists
                activityTracker.Track("Configuration does not contain a serviceModel section. Message forwarding is disabled.");

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

        private void ProcessMessageInternal(HyperNodeTaskInfo args)
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

                ICommandRequest commandRequest;
                try
                {
                    // Deserialize the request string
                    commandRequest = requestSerializer.Deserialize(args.Message.CommandRequestString);
                }
                catch (Exception ex)
                {
                    throw new InvalidCommandRequestTypeException(
                        string.Format(
                            "Command '{0}' expected a request type of '{1}', but the command request string could not be deserialized into that type. See inner exception for details.",
                            args.Message.CommandName,
                            requestSerializer.GetType().FullName
                        ),
                        ex
                    );
                }

                try
                {
                    // Create the execution context to pass into our module
                    var context = new CommandExecutionContext(args.Message.IntendedRecipientNodeNames,
                        args.Message.SeenByNodeNames)
                    {
                        TaskId = args.Response.TaskId,
                        ExecutingNodeName = this.HyperNodeName,
                        CommandName = args.Message.CommandName,
                        CreatedByAgentName = args.Message.CreatedByAgentName,
                        CreationDateTime = args.Message.CreationDateTime,
                        ProcessOptionFlags = args.Message.ProcessOptionFlags,
                        Request = commandRequest,
                        Activity = args.Activity,
                        Token = args.Token
                    };

                    // Execute the command
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
                args.Activity.TrackFormat("Fatal error: Invalid command '{0}'.", args.Message.CommandName);
            }

            // Make sure we report cancellation if it was requested
            if (args.Token.IsCancellationRequested)
                commandResponse.ProcessStatusFlags |= MessageProcessStatusFlags.Cancelled;

            args.Response.ProcessStatusFlags = commandResponse.ProcessStatusFlags;
        }

        private void AddCommandModuleConfiguration(CommandModuleConfiguration commandConfig)
        {
            if (commandConfig == null)
                throw new ArgumentNullException("commandConfig");

            if (string.IsNullOrWhiteSpace(commandConfig.CommandName))
                throw new ArgumentException("The CommandName property of the commandConfig parameter must not be null or whitespace.", "commandConfig");

            if (!_commandModuleConfigurations.TryAdd(commandConfig.CommandName, commandConfig))
            {
                throw new DuplicateCommandException(
                    string.Format("A command already exists with the name '{0}'.", commandConfig.CommandName)
                );
            }
        }

        /// <summary>
        /// Removes the task with the specified <paramref name="taskId"/> from the internal dictionary of tasks and calls Dispose() on it.
        /// </summary>
        /// <param name="taskId">The ID of the task to clean up.</param>
        private void TaskCleanUp(string taskId)
        {
            // Remove our task info and dispose of it
            HyperNodeTaskInfo taskInfo;
            if (_liveTasks.TryRemove(taskId, out taskInfo) && taskInfo != null)
                taskInfo.Dispose();
        }

        private IEnumerable<Task> GetChildTasks()
        {
            return _liveTasks.Keys.Select(
                taskId => _liveTasks[taskId].WhenChildTasks()
            );
        }

        #endregion Private Methods
    }
}
