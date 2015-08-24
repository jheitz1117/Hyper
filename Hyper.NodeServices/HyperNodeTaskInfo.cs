﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices
{
    internal sealed class HyperNodeTaskInfo : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly CancellationToken _masterToken;
        private CancellationTokenSource _taskTokenSource;
        private readonly List<Task> _childTasks = new List<Task>();

        #region Properties

        public CancellationToken Token
        {
            get
            {
                if (_taskTokenSource == null)
                    _taskTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_masterToken);

                return _taskTokenSource.Token;
            }
        }

        public HyperNodeServiceActivityTracker Activity { get; set; }

        private readonly CompositeDisposable _activitySubscribers = new CompositeDisposable();
        public CompositeDisposable ActivitySubscribers
        {
            get { return _activitySubscribers; }
        }

        public HyperNodeMessageRequest Message { get; set; }
        
        public HyperNodeMessageResponse Response { get; set; }

        public TimeSpan Elapsed
        {
            get { return _stopwatch.Elapsed; }
        }

        #endregion Properties

        #region Public Methods

        public HyperNodeTaskInfo(CancellationToken masterToken)
        {
            _masterToken = masterToken;
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
                this.Response.TotalRunTime = _stopwatch.Elapsed;

                /* Signal completion before we dispose our subscribers. This is necessary because clients who are polling the service for progress
                 * updates must know when the service is done sending updates. Make sure we pass the final, completed response object in case we have
                 * any monitors that are watching for it. */
                if (this.Activity != null)
                    this.Activity.TrackFinished(this.Response);

                if (this.ActivitySubscribers != null)
                    this.ActivitySubscribers.Dispose();

                if (_taskTokenSource != null)
                    _taskTokenSource.Dispose();
            }
        }

        #endregion Private Methods
    }
}
