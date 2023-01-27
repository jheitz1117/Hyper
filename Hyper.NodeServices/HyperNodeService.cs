using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Hyper.ActivityTracking;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.ActivityTracking.Monitors;
using Hyper.NodeServices.ActivityTracking.Trackers;
using Hyper.NodeServices.CommandModules;
using Hyper.NodeServices.Configuration;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.EventTracking;
using Hyper.NodeServices.Exceptions;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Configuration;
using Hyper.NodeServices.Extensibility.EventTracking;

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

        private static readonly ICommandRequestSerializer DefaultRequestSerializer = new PassThroughSerializer();
        private static readonly ICommandResponseSerializer DefaultResponseSerializer = new PassThroughSerializer();
        private static readonly IHyperNodeConfigurationProvider DefaultConfigurationProvider = new HyperNodeSectionConfigurationProvider();

        #endregion Defaults

        #region Private Members

        private static readonly object Lock = new object();

        private readonly TaskProgressCacheMonitor _taskProgressCacheMonitor = new TaskProgressCacheMonitor();
        private readonly List<HyperNodeServiceActivityMonitor> _customActivityMonitors = new List<HyperNodeServiceActivityMonitor>();
        private readonly ConcurrentDictionary<string, CommandModuleConfiguration> _commandModuleConfigurations = new ConcurrentDictionary<string, CommandModuleConfiguration>();
        private readonly CancellationTokenSource _masterTokenSource = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, HyperNodeTaskInfo> _liveTasks = new ConcurrentDictionary<string, HyperNodeTaskInfo>();

        #endregion Private Members

        #region Properties

        private string HyperNodeName { get; }

        internal bool EnableTaskProgressCache
        {
            get => _taskProgressCacheMonitor.Enabled;
            set => _taskProgressCacheMonitor.Enabled = value;
        }

        internal TimeSpan TaskProgressCacheDuration
        {
            get => _taskProgressCacheMonitor.CacheDuration;
            set => _taskProgressCacheMonitor.CacheDuration = value;
        }

        private ITaskIdProvider TaskIdProvider { get; set; }
        private IHyperNodeEventHandler EventHandler { get; set; }
        internal bool EnableDiagnostics { get; set; }
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
        public async Task<HyperNodeMessageResponse> ProcessMessageAsync(HyperNodeMessageRequest message)
        {
            var response = new HyperNodeMessageResponse(HyperNodeName)
            {
                NodeAction = HyperNodeActionType.None,
                NodeActionReason = HyperNodeActionReasonType.Unknown,
                ProcessStatusFlags = MessageProcessStatusFlags.None
            };

            var currentTaskInfo = new HyperNodeTaskInfo(HyperNodeName, _masterTokenSource.Token, message, response, message.ReturnTaskTrace || EnableDiagnostics);

            // Start our task-level stopwatch to track total time. If diagnostics are disabled, calling this method has no effect.
            currentTaskInfo.StartStopwatch();

            // This tracker is used to temporarily store activity event items until we know what to do with them
            var queueTracker = new TaskActivityQueueTracker(currentTaskInfo);
            
            // Check if we should reject this request
            var rejectionReason = GetRejectionReason(currentTaskInfo, queueTracker);
            if (rejectionReason.HasValue)
            {
                response.NodeAction = HyperNodeActionType.Rejected;
                response.NodeActionReason = rejectionReason.Value;
                response.ProcessStatusFlags = MessageProcessStatusFlags.Cancelled;
                response.TaskId = null;

                // At this point in the game, we don't have a "real" activity tracker yet because we haven't initialized it. So instead of
                // running the events through the activity monitors as we normally would, we'll just add the information to the task trace
                // of the response object and send that back so the caller knows what happened.
                while (queueTracker.Count > 0)
                {
                    var activityItem = queueTracker.Dequeue();

                    response.TaskTrace.Add(
                        new HyperNodeActivityItem
                        {
                            Agent = activityItem.Agent,
                            EventDateTime = activityItem.EventDateTime,
                            EventDetail = activityItem.EventDetail,
                            EventDescription = activityItem.EventDescription,
                            ProgressPart = activityItem.ProgressPart,
                            ProgressTotal = activityItem.ProgressTotal,
                            Elapsed = activityItem.Elapsed
                        }
                    );
                }

                // If the message was rejected, then the HyperNodeTaskInfo was not added to the list of live events, which means it will never be disposed unless we do so here.
                // This will also set the TotalRunTime on the response so we know how long it took to reject the message
                currentTaskInfo.Dispose();
            }
            else
            {
                // Initialize our activity tracker so we can track progress
                InitializeActivityTracker(currentTaskInfo);

                // Now that we've setup our "real" activity tracker, let's report all the activity we've stored up until now
                while (queueTracker.Count > 0)
                {
                    // If the item doesn't have a task ID, set it here.
                    var activityItem = queueTracker.Dequeue();
                    if (string.IsNullOrWhiteSpace(activityItem.TaskId))
                        activityItem.TaskId = currentTaskInfo.TaskId;

                    // Now track it verbatim (all info is preserved, i.e. the original date/time of the event and other properties)
                    currentTaskInfo.Activity.TrackActivityVerbatim(activityItem);
                }

                #region Process Message

                try
                {
                    // Track that we've started a task to process the message.
                    currentTaskInfo.Activity.TrackTaskStarted();

                    // Check if user cancelled the message in the OnTaskStarted event
                    currentTaskInfo.Token.ThrowIfCancellationRequested();

                    // This node accepts responsibility for processing this message
                    response.NodeAction = HyperNodeActionType.Accepted;
                    response.NodeActionReason = HyperNodeActionReasonType.ValidMessage;
                    currentTaskInfo.Activity.Track("Attempting to process message...");

                    // Define the method in a safe way (i.e. with a try/catch around it)
                    var processMessageInternalSafe = new Func<HyperNodeTaskInfo, Task>(
                        async args =>
                        {
                            try
                            {
                                await ProcessMessageInternal(args).ConfigureAwait(false);
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
                                args.Activity.TrackMessageProcessed();
                            }
                        }
                    );

                    if (message.RunConcurrently)
                    {
                        currentTaskInfo.AddChildTask(
                            Task.Run(
                                async () => await processMessageInternalSafe(currentTaskInfo).ConfigureAwait(false),
                                currentTaskInfo.Token
                            )
                        );
                    }
                    else
                    {
                        await processMessageInternalSafe(currentTaskInfo).ConfigureAwait(false);
                    }

                    // Now that we have all of our tasks doing stuff, we need to make sure we clean up after ourselves.
                    if (message.RunConcurrently)
                    {
                        // If we're running concurrently, we want to return immediately and allow the clean up to occur as a continuation after the tasks are finished.
                        // IMPORTANT: We're deliberately not using await here because we *want* to return asap because the user requested the command be run concurrently.
                        currentTaskInfo.WhenChildTasks().ContinueWith(t => TaskCleanUp(currentTaskInfo.TaskId));
                    }
                    else
                    {
                        // Otherwise, if we're running synchronously, we want to block until all our child threads are done, then clean up.
                        currentTaskInfo.WaitChildTasks(_masterTokenSource.Token);
                        TaskCleanUp(currentTaskInfo.TaskId);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException)
                        response.ProcessStatusFlags |= MessageProcessStatusFlags.Cancelled;
                    else
                        response.ProcessStatusFlags = MessageProcessStatusFlags.Failure;

                    currentTaskInfo.Activity.TrackException(ex);

                    // Make sure we clean up our task if any exceptions were thrown
                    TaskCleanUp(currentTaskInfo.TaskId);
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
        /// implementations defined.
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
            (TaskIdProvider as IDisposable)?.Dispose();

            // Dispose of our task progress cache monitor
            _taskProgressCacheMonitor?.Dispose();

            // Dispose of our master cancellation token source
            _masterTokenSource?.Dispose();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Initializes an instance of the <see cref="HyperNodeService"/> class with the specified name.
        /// </summary>
        /// <param name="hyperNodeName">The name of the <see cref="HyperNodeService"/>.</param>
        private HyperNodeService(string hyperNodeName)
        {
            HyperNodeName = hyperNodeName;
        }

        private void InitializeActivityTracker(HyperNodeTaskInfo currentTaskInfo)
        {
            currentTaskInfo.Activity = new HyperNodeTaskActivityTracker(
                currentTaskInfo,
                EventHandler,

                // Possible user actions
                currentTaskInfo.Cancel
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

                var systemActivityMonitors = new List<HyperNodeServiceActivityMonitor>();
                /*****************************************************************************************************************
                 * Subscribe our task progress cache monitor to our event stream only if the client requested it and the feature is actually enabled. There is currently no built-in
                 * functionality to support long-running task tracing other than the memory cache. If the client opts to disable the memory cache to save resources,
                 * then they will need to setup a custom HyperNodeServiceActivityMonitor if they still want to be able to know what's going on in the server. Custom
                 * HyperNodeServiceActivityMonitor objects can record activity to a database or some other data store, which the user can then query for the desired
                 * activity.
                 *****************************************************************************************************************/
                if (EnableTaskProgressCache && currentTaskInfo.Message.CacheTaskProgress)
                    systemActivityMonitors.Add(_taskProgressCacheMonitor);

                /*****************************************************************************************************************
                 * If a task trace was requested, we're only going to honor the request if the task runs synchronously. In the first
                 * place, the response task trace will be incomplete for concurrent tasks anyway, but I eventually learned that
                 * this response task trace monitor can enter into a race condition against the serializer when the response is
                 * returned. Effectively, if the task trace enumeration is modified (by this monitor) while it is being serialized
                 * (which is very possible), then the serializer throws an exception which shows up as an obscure communication
                 * exception from the client's point of view. This was very hard to track down, requiring use of WCF's advanced
                 * tracing tools.
                 *****************************************************************************************************************/
                if (currentTaskInfo.Message.ReturnTaskTrace && !currentTaskInfo.Message.RunConcurrently)
                    systemActivityMonitors.Add(new ResponseTaskTraceMonitor(currentTaskInfo.Response));

                /*****************************************************************************************************************
                 * Subscribe our system activity monitors to the event stream first
                 *****************************************************************************************************************/
                currentTaskInfo.ActivityObservers.AddRange(
                    systemActivityMonitors.Select(
                        monitor => new HyperNodeActivityObserver(
                            monitor,
                            liveEvents,
                            Scheduler.CurrentThread,
                            currentTaskInfo
                        )
                    )
                );

                /*****************************************************************************************************************
                 * Subscribe our custom activity monitors to the event stream last, just in case any of them throw exceptions. If they do, we've already setup our
                 * task trace and cache monitors at this point, so we can actually use the activity tracker to track the exceptions. This way, we can make sure that
                 * any exceptions thrown by user code are available to be reported to the client.
                 *
                 * Also, see the following for discussions about the various schedulers:
                 *     http://stackoverflow.com/questions/25993482/creating-an-sta-thread-when-using-reactive-extensions-rx-schedulers
                 *         - Difference between NewThreadScheduler and EventLoopScheduler
                 *     http://stackoverflow.com/questions/26159965/rx-taskpoolscheduler-vs-eventloopscheduler-memory-usage
                 *         - Excessive memory usage from TaskPoolScheduler
                 * As far as these schedulers go, I have decided to force all activity monitors to go through the current thread. This is because I don't want to
                 * blow up the server with all kinds of extra threads. Right now, I feel that if the activity monitors are slowing down the server to the point
                 * that we have to introduce some concurrency, then the caller can just indicate in the request object to run the task concurrently, so that the
                 * extra time consumed by the activity monitors is absorbed into the more general task thread.
                 *****************************************************************************************************************/
                currentTaskInfo.ActivityObservers.AddRange(
                    _customActivityMonitors
                        .Where(m => m.Enabled) // Only add the monitors that are enabled
                        .Select(
                            monitor => new HyperNodeActivityObserver(
                                monitor,
                                liveEvents,
                                Scheduler.CurrentThread,
                                currentTaskInfo
                            )
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

        private HyperNodeActionReasonType? GetRejectionReason(HyperNodeTaskInfo taskInfo, ITaskActivityTracker queueTracker)
        {
            HyperNodeActionReasonType? rejectionReason = null;
            
            HyperNodeActivityItem userRejectionActivity = null;
            try
            {
                // Allow the user to reject the message if necessary
                EventHandler.OnMessageReceived(
                    new MessageReceivedEventArgs(
                        queueTracker,
                        taskInfo,
                        r =>
                        {
                            rejectionReason = HyperNodeActionReasonType.Custom;
                            userRejectionActivity = new HyperNodeActivityItem(HyperNodeName)
                            {
                                EventDescription = "The message was rejected by user-defined code. See the EventDetail property for the rejection reason.",
                                EventDetail = r
                            };
                        }
                    )
                );
            }
            catch (Exception ex)
            {
                // User-defined event handler threw an exception, so assume the worst and reject the message.
                rejectionReason = HyperNodeActionReasonType.Custom;
                userRejectionActivity = new HyperNodeActivityItem(HyperNodeName)
                {
                    EventDescription = "An exception was thrown by user-defined code while processing the OnMessageReceived event. See the EventDetail property for the exception details.",
                    EventDetail = ex.ToString()
                };
            }

            // If the user didn't reject the message, give the system a chance to reject it
            var systemRejectionActivity = new HyperNodeActivityItem(HyperNodeName);
            if (!rejectionReason.HasValue)
            {
                if (MaxConcurrentTasks > -1 && _liveTasks.Count >= MaxConcurrentTasks)
                {
                    rejectionReason = HyperNodeActionReasonType.MaxConcurrentTaskCountReached;
                    systemRejectionActivity.EventDescription = $"The maximum number of concurrent tasks ({MaxConcurrentTasks}) has been reached.";
                    systemRejectionActivity.EventDetail = "An attempted was made to start a new task when the maximum number of concurrent tasks were already running. Consider increasing the value of MaxConcurrentTasks or setting it to -1 (unlimited).";
                }
                else if (_masterTokenSource.IsCancellationRequested)
                {
                    rejectionReason = HyperNodeActionReasonType.CancellationRequested;
                    systemRejectionActivity.EventDescription = "The service is shutting down. No new tasks can be started.";
                    systemRejectionActivity.EventDetail = "The service-level cancellation token has been triggered. No new tasks are being spun up and all existing tasks are in the process of shutting down.";
                }
                else
                {
                    // If we get this far, there's a good chance the message will not be rejected. However, if the ITaskIdProvider is ill-behaved (throws an exception), then we
                    // may still have to reject the message due to being unable to get a task ID

                    string taskId = null;
                    
                    try
                    {
                        // Try to use our custom task ID provider
                        taskId = TaskIdProvider.CreateTaskId(taskInfo.MessageInfo);
                    }
                    catch (Exception ex)
                    {
                        // Failed to get a Task ID, so reject the message
                        rejectionReason = HyperNodeActionReasonType.TaskIdProviderThrewException;
                        systemRejectionActivity.EventDescription = "An exception was thrown while attempting to create a task ID.";
                        systemRejectionActivity.EventDetail = ex.ToString();
                    }

                    if (string.IsNullOrWhiteSpace(taskId))
                    {
                        rejectionReason = HyperNodeActionReasonType.InvalidTaskId;
                        systemRejectionActivity.EventDescription = $"The class '{TaskIdProvider.GetType().FullName}' created a blank task ID.";
                    }
                    else if (!_liveTasks.TryAdd(taskId, taskInfo))
                    {
                        rejectionReason = HyperNodeActionReasonType.DuplicateTaskId;
                        systemRejectionActivity.EventDescription = $"A task with ID '{taskId}' is already running.";
                        systemRejectionActivity.EventDetail = "A duplicate task ID was generated. This can occur when a new instance of singleton task is started while an existing instance of that task is running. If you know the task will complete, please try again after the task is finished. You may also consider cancelling the task and then restarting it.";
                    }
                    else
                    {
                        // In this case we don't want to reject the message after all
                        systemRejectionActivity = null;

                        // Don't forget to set the task ID in our task info object
                        taskInfo.TaskId = taskId;
                    }
                }
            }

            // Set the rejection activity here, and let the user-defined rejection take precedence over the system-defined rejection
            var effectiveRejectionActivity = userRejectionActivity ?? systemRejectionActivity;

            // Finally, queue up the rejection activity last, after all of the user's other events they may have tracked
            if (effectiveRejectionActivity != null)
                queueTracker.Track(effectiveRejectionActivity.EventDescription, effectiveRejectionActivity.EventDetail);

            return rejectionReason;
        }

        private async Task ProcessMessageInternal(HyperNodeTaskInfo args)
        {
            ICommandResponse commandResponse;

            if (_commandModuleConfigurations.ContainsKey(args.Message.CommandName) &&
                _commandModuleConfigurations.TryGetValue(args.Message.CommandName, out var commandModuleConfig) &&
                commandModuleConfig.Enabled)
            {
                // Create our command module instance
                var commandInstance = Activator.CreateInstance(commandModuleConfig.CommandModuleType);

                ICommandRequestSerializer requestSerializer = null;
                ICommandResponseSerializer responseSerializer = null;

                // Use the factories to create serializers, if applicable
                if (commandInstance is ICommandRequestSerializerFactory requestSerializerFactory)
                    requestSerializer = requestSerializerFactory.Create();
                if (commandInstance is ICommandResponseSerializerFactory responseSerializerFactory)
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
                        $"Command '{args.Message.CommandName}' expected a request type of '{requestSerializer.GetType().FullName}', but the command request string could not be deserialized into that type. See inner exception for details.",
                        ex
                    );
                }

                try
                {
                    // Create the execution context to pass into our module
                    var context = new CommandExecutionContext
                    {
                        TaskId = args.Response.TaskId,
                        ExecutingNodeName = HyperNodeName,
                        CommandName = args.Message.CommandName,
                        CreatedByAgentName = args.Message.CreatedByAgentName,
                        ProcessOptionFlags = args.Message.ProcessOptionFlags,
                        Request = commandRequest,
                        Activity = args.Activity,
                        Token = args.Token
                    };

                    // Execute the command
                    if (commandInstance is IAwaitableCommandModule awaitableCommand)
                        commandResponse = await awaitableCommand.Execute(context).ConfigureAwait(false);
                    else
                        commandResponse = ((ICommandModule)commandInstance).Execute(context);

                    // Serialize the response to send back
                    args.Response.CommandResponseString = responseSerializer.Serialize(commandResponse);
                }
                finally
                {
                    // Check if our module is disposable and take care of it appropriately
                    (commandInstance as IDisposable)?.Dispose();
                }
            }
            else
            {
                // Unrecognized command
                commandResponse = new CommandResponse(MessageProcessStatusFlags.Failure | MessageProcessStatusFlags.InvalidCommand);
                args.Activity.Track($"Fatal error: Invalid {nameof(args.Message.CommandName)} '{args.Message.CommandName}'.");
            }

            // Make sure we report cancellation if it was requested
            if (args.Token.IsCancellationRequested)
                commandResponse.ProcessStatusFlags |= MessageProcessStatusFlags.Cancelled;

            args.Response.ProcessStatusFlags = commandResponse.ProcessStatusFlags;
        }

        private void AddCommandModuleConfiguration(CommandModuleConfiguration commandConfig)
        {
            if (commandConfig == null)
                throw new ArgumentNullException(nameof(commandConfig));

            if (string.IsNullOrWhiteSpace(commandConfig.CommandName))
                throw new ArgumentException($"The {nameof(commandConfig.CommandName)} property of the {nameof(commandConfig)} parameter must not be null or whitespace.", nameof(commandConfig));

            if (!_commandModuleConfigurations.TryAdd(commandConfig.CommandName, commandConfig))
            {
                throw new DuplicateCommandException(
                    $"A command already exists with the {nameof(commandConfig.CommandName)} '{commandConfig.CommandName}'."
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
            if (_liveTasks.TryRemove(taskId, out var taskInfo))
                taskInfo?.Dispose();
        }

        private IEnumerable<Task> GetChildTasks()
        {
            return _liveTasks.Keys.Select(
                taskId => _liveTasks.TryGetValue(taskId, out var task)
                    ? task.WhenChildTasks()
                    : Task.CompletedTask
            );
        }

        #endregion Private Methods
    }
}
