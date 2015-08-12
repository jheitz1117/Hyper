using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using Hyper.NodeServices.ActivityTracking;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices
{
    internal sealed class HyperNodeTaskInfo : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly CancellationToken _masterToken;
        private CancellationTokenSource _taskTokenSource;

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
