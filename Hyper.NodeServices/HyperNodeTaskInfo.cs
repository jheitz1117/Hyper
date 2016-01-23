using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices
{
    internal sealed class HyperNodeTaskInfo : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly CancellationTokenSource _taskTokenSource;
        private readonly List<Task> _childTasks = new List<Task>();

        #region Properties

        public string TaskId
        {
            get { return Response.TaskId; }
            set { Response.TaskId = value; }
        }

        public CancellationToken Token => _taskTokenSource.Token;

        public HyperNodeTaskActivityTracker Activity { get; set; }

        public IConnectableObservable<Unit> TerminatingSequence { get; } = Observable.Return(Unit.Default).Publish();

        public CompositeDisposable ActivitySubscribers { get; } = new CompositeDisposable();

        public HyperNodeMessageRequest Message { get; }

        public HyperNodeMessageResponse Response { get; }

        public TimeSpan Elapsed => _stopwatch.Elapsed;

        #endregion Properties

        #region Public Methods

        public HyperNodeTaskInfo(CancellationToken masterToken, HyperNodeMessageRequest message, HyperNodeMessageResponse response)
        {
            _taskTokenSource = CancellationTokenSource.CreateLinkedTokenSource(masterToken);
            Message = message;
            Response = response;
        }

        public void StartStopwatch()
        {
            if (!_stopwatch.IsRunning)
                _stopwatch.Start();
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="childTask">The object to be added to the end of the <see cref="List{T}"/>. The value can be null.</param>
        public void AddChildTask(Task childTask)
        {
            _childTasks.Add(childTask);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="List{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="List{T}"/>. The collection itself cannot be null, but it can contain elements that are null.</param>
        public void AddChildTasks(IEnumerable<Task> collection)
        {
            _childTasks.AddRange(collection);
        }

        /// <summary>
        /// Creates a task that will complete when all of the child tasks have completed.
        /// </summary>
        /// <returns></returns>
        public Task WhenChildTasks()
        {
            return Task.WhenAll(_childTasks);
        }

        /// <summary>
        /// Waits for all of the child <see cref="Task"/> objects to complete execution.
        /// </summary>
        /// <param name="token">A <see cref="CancellationToken"/> to observe while waiting for the tasks to complete.</param>
        /// <returns></returns>
        public void WaitChildTasks(CancellationToken token)
        {
            Task.WaitAll(_childTasks.ToArray(), token);
        }

        public void Cancel()
        {
            if (!_taskTokenSource.IsCancellationRequested)
                _taskTokenSource.Cancel();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion Public Methods

        #region Private Methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // First, stop our stopwatch so we can record the total elapsed time for this task
                _stopwatch.Stop();
                Response.TotalRunTime = _stopwatch.Elapsed;

                /* Signal completion before we dispose our subscribers. This is necessary because clients who are polling the service for progress
                 * updates must know when the service is done sending updates. Make sure we pass the final, completed response object in case we have
                 * any monitors that are watching for it. */
                Activity?.TrackTaskComplete(Response);

                // Signal that we are done raising activity events to ensure that the queues for all of our schedulers don't keep having stuff appended to the end
                // This also triggers the OnComplete() event for all subscribers, which should automatically trigger the scheduling of their disposal
                TerminatingSequence.Connect().Dispose();

                /*
                 * If we dispose of our activity subscribers at this point, it's possible that some subscribers may be disposed before they've processed all of their queued items.
                 * Currently, the only way I can think of for the disposal to NOT happen is if someone writes an infinite loop into their observer to prevent the OnNext() from
                 * returning. If they've done this, I'm hoping the problem will be fairly obvious to the user. As it stands, I will not be manually disposing the activity
                 * subscribers at this time; I will allow the automatic disposal scheduling to take care of it.
                 */

                _taskTokenSource?.Dispose();
            }
        }

        #endregion Private Methods
    }
}
