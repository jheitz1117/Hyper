﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Hyper.ActivityTracking;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.ActivityTracking.Trackers;
using Hyper.NodeServices.CommandModules;
using Hyper.NodeServices.Configuration;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
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
            get { return _taskProgressCacheMonitor.Enabled; }
            set { _taskProgressCacheMonitor.Enabled = value; }
        }

        internal TimeSpan TaskProgressCacheDuration
        {
            get { return _taskProgressCacheMonitor.CacheDuration; }
            set { _taskProgressCacheMonitor.CacheDuration = value; }
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
        public HyperNodeMessageResponse ProcessMessage(HyperNodeMessageRequest message)
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

                    if (message.ExpirationDateTime <= DateTime.Now)
                    {
                        // Message has expired, so ignore it and do not forward it
                        response.NodeAction = HyperNodeActionType.Ignored;
                        response.NodeActionReason = HyperNodeActionReasonType.MessageExpired;
                        currentTaskInfo.Activity.TrackMessageIgnored("Message expired on " + message.ExpirationDateTime.ToString("G") + ".");
                    }
                    else if (message.SeenByNodeNames.Contains(HyperNodeName))
                    {
                        // Message was already processed by this node, so ignore it and do not forward it (since it would have been forwarded the first time)
                        response.NodeAction = HyperNodeActionType.Ignored;
                        response.NodeActionReason = HyperNodeActionReasonType.PreviouslySeen;
                        currentTaskInfo.Activity.TrackMessageIgnored("Message previously seen.");
                    }
                    else
                    {
                        // Add this node to the list of nodes who have seen this message
                        message.SeenByNodeNames.Add(HyperNodeName);
                        currentTaskInfo.Activity.TrackMessageSeen();

                        // Check if user cancelled the message in the OnMessageSeen event
                        currentTaskInfo.Token.ThrowIfCancellationRequested();

                        // Check if this message has a list of intended recipients, and if this node was one of them.
                        // An empty recipients list means means the message is indended for all nodes in the forwarding path.
                        if (message.IntendedRecipientNodeNames.Any() && !message.IntendedRecipientNodeNames.Contains(HyperNodeName))
                        {
                            // This node was not an intended recipient, so ignore the message, but still forward it if possible.
                            response.NodeAction = HyperNodeActionType.Ignored;
                            response.NodeActionReason = HyperNodeActionReasonType.UnintendedRecipient;
                            currentTaskInfo.Activity.TrackMessageIgnored("Message not intended for agent.");
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
                                        args.Activity.TrackMessageProcessed();
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
                 * If we want a task trace returned, that's fine, but the task trace in the real-time response would only ever contain events recorded during the current
                 * call to ProcessMessage() as opposed to the entire lifetime of a task in general. This is because a task can be spawned in a child thread and can outlive
                 * the original call to ProcessMessage(). This is the whole reason why we have a TaskProgressCacheMonitor that is in charge of collecting trace information
                 * for tasks that run concurrently. It should be noted that if the client elects to use caching AND return a task trace, then the events recorded in the
                 * real-time task trace and those recorded in the cache will likely overlap for events which fired before ProcessMessage() returned. However, any events that
                 * fired after ProcessMessage() returned would only be recorded in the cache. This may result in a task trace that looks incomplete since the "processing
                 * complete" message would not have occured before the method returned.
                 *****************************************************************************************************************/
                if (currentTaskInfo.Message.ReturnTaskTrace)
                    systemActivityMonitors.Add(new ResponseTaskTraceMonitor(currentTaskInfo.Response));

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
                systemActivityMonitors.Add(new ChildNodeResponseMonitor(currentTaskInfo.Response));

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

        /// <summary>
        /// Forwards the specified <see cref="HyperNodeMessageRequest"/> object using the specified <see cref="HyperNodeTaskInfo"/> object.
        /// </summary>
        /// <param name="taskInfo">The <see cref="HyperNodeTaskInfo"/> object containing the information to forward.</param>
        /// <returns></returns>
        private IEnumerable<Task> ForwardMessage(HyperNodeTaskInfo taskInfo)
        {
            Task[] forwardingTasks = { };

            // Allow a null path to be passed. We can just treat it as an empty path and not forward the message to anyone
            var remoteNodes = GetConnectedHyperNodes(
                (taskInfo.Message.ForwardingPath ?? new HyperNodePath()).GetChildren(HyperNodeName).ToList(),
                taskInfo.Activity
            );

            if (remoteNodes != null)
            {
                forwardingTasks = remoteNodes.Select(
                    remoteNode => new ForwardingTaskParameter(remoteNode.Key, taskInfo)
                ).Select(
                    childForwardingParam =>
                    {
                        // Track the message forwarding. This also gives the user a chance to skip a recipient if they wish.
                        var shouldForward = true;
                        childForwardingParam.TaskInfo.Activity.TrackForwardingMessage(
                            childForwardingParam.RemoteNodeName,
                            () =>
                            {
                                shouldForward = false;
                                childForwardingParam.TaskInfo.Activity.Track($"The message was not forwarded to HyperNode '{childForwardingParam.RemoteNodeName}' because it was skipped by a user-defined event handler.");
                            }
                        );

                        // Check if user tried to cancel the task
                        taskInfo.Token.ThrowIfCancellationRequested();

                        Task childTask = null;
                        if (shouldForward)
                        {
                            childTask = Task.Factory.StartNew(
                                param =>
                                {
                                    var forwardingParam = (ForwardingTaskParameter) param;
                                    HyperNodeMessageResponse response = null;

                                    try
                                    {
                                        // By the time we get here, we have guaranteed that a client endpoint exists with the specified name.
                                        using (var client = new HyperNodeClient(forwardingParam.RemoteNodeName))
                                        {
                                            response = client.ProcessMessage(forwardingParam.TaskInfo.Message);
                                        }
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

                                    // The response might be null if the user chose to skip this recipient.
                                    return response;
                                },
                                childForwardingParam,
                                taskInfo.Token
                            ).ContinueWith(
                                (forwardingTask, param) =>
                                {
                                    var forwardingParam = (ForwardingTaskParameter) param;

                                    // Check the status of the antecedent and report accordingly
                                    switch (forwardingTask.Status)
                                    {
                                        case TaskStatus.RanToCompletion:
                                            if (forwardingTask.Result != null)
                                            {
                                                forwardingParam.TaskInfo.Activity.TrackHyperNodeResponded(
                                                    forwardingParam.RemoteNodeName, forwardingTask.Result
                                                );
                                            }
                                            break;
                                        case TaskStatus.Faulted:
                                            forwardingParam.TaskInfo.Activity.Track(
                                                $"An unhandled exception was thrown by HyperNode '{forwardingParam.RemoteNodeName}' as a fault.",
                                                "This is normally caused by an error thrown in the ProcessMessage() method, but could also be caused by an extension or plugin. Exception detail follows:\r\n" +
                                                forwardingTask.Exception
                                            );
                                            break;
                                        default:
                                            forwardingParam.TaskInfo.Activity.Track(
                                                $"Child task to forward message to HyperNode '{forwardingParam.RemoteNodeName}' completed with status '{forwardingTask.Status.ToString()}'."
                                            );
                                            break;
                                    }
                                },
                                childForwardingParam,
                                taskInfo.Token
                            );
                        }

                        // This could be null if the user opted to skip the recipient. If so, then we'll just weed out the nulls below
                        return childTask;
                    }
                ).Where(
                    t => t != null // Weed out the null tasks that result from skipping recipients
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
                            $"HyperNode '{forwardingArgs.RemoteNodeName}' timed out at {taskInfo.Message.ForwardingTimeout}.",
                            string.Join(
                                " ",
                                $"HyperNode '{forwardingArgs.RemoteNodeName}' did not return a response before the specified timeout of {taskInfo.Message.ForwardingTimeout}. This is usually caused by a long-running task in the",
                                "recipient node or one of its descendants. Consider increasing the value of the ForwardingTimeout property in the message or lengthening",
                                "the message timeout attributes in your WCF configuration. If caching was enabled for this message, additional trace logs may be obtained",
                                "by querying the intended recipients directly. Additional trace logs may also be obtained by querying the data stores for any custom",
                                "activity monitors that may be attached to the intended recipients."
                            )
                        );
                    }
                }
            }

            return forwardingTasks;
        }

        private static IEnumerable<HyperNodeVertex> GetConnectedHyperNodes(List<HyperNodeVertex> vertices, ITaskActivityTracker activityTracker)
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
                            activityTracker.Track($"Message could not be forwarded to HyperNode '{v.Key}' because no client endpoint was found with that name.");

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
                        $"Command '{args.Message.CommandName}' expected a request type of '{requestSerializer.GetType().FullName}', but the command request string could not be deserialized into that type. See inner exception for details.",
                        ex
                    );
                }

                try
                {
                    // Create the execution context to pass into our module
                    var context = new CommandExecutionContext(args.Message.IntendedRecipientNodeNames, args.Message.SeenByNodeNames)
                    {
                        TaskId = args.Response.TaskId,
                        ExecutingNodeName = HyperNodeName,
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
                    (commandModule as IDisposable)?.Dispose();
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
            HyperNodeTaskInfo taskInfo;
            if (_liveTasks.TryRemove(taskId, out taskInfo))
                taskInfo?.Dispose();
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
