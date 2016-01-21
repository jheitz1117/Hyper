using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    /// <summary>
    /// When disposed, uses the specified <see cref="IScheduler"/> to schedule the disposal of itself and the specified <see cref="HyperNodeServiceActivityMonitor"/>.
    /// </summary>
    internal sealed class HyperNodeActivityObserver : IObserver<IHyperNodeActivityEventItem>, IDisposable
    {
        private readonly HyperNodeServiceActivityMonitor _underlyingMonitor;
        private readonly ITaskActivityTracker _activity;
        private readonly IDisposable _subscription;
        private readonly IScheduler _scheduler;
        private bool _isDisposed;

        public HyperNodeActivityObserver(HyperNodeServiceActivityMonitor underlyingMonitor, IObservable<IHyperNodeActivityEventItem> activityEventStream, IScheduler scheduler, HyperNodeTaskInfo taskInfo)
        {
            _underlyingMonitor = underlyingMonitor;
            _activity = taskInfo.Activity;
            _scheduler = scheduler;
            _subscription = activityEventStream
                .Where(
                    e =>
                    {
                        var shouldTrack = false;

                        try
                        {
                            // ...for activity items matching the specified criteria
                            shouldTrack = underlyingMonitor.ShouldTrack(e);
                        }
                        catch (Exception ex)
                        {
                            // This is legal at this point because we already subscribed our cache and task trace monitors earlier
                            _activity.TrackException(
                                new ActivityMonitorSubscriptionException(
                                    string.Format(
                                        "Unable to subscribe activity monitor '{0}' because its ShouldTrack() method threw an exception.",
                                        underlyingMonitor.Name
                                    ),
                                ex
                                )
                            );
                        }

                        return shouldTrack;
                    }
                ).TakeUntil(taskInfo.TerminatingSequence)
                .ObserveOn(scheduler)
                .SubscribeSafe(this);
        }

        public void OnNext(IHyperNodeActivityEventItem activity)
        {
            try
            {
                _underlyingMonitor.OnTrack(activity);
            }
            catch (Exception ex)
            {
                // If the user-defined OnTrack() method throws an exception, kill the subscription immediately (and have our scheduler schedule itself for disposal) to prevent further exceptions from being thrown...
                if (_subscription != null)
                    _subscription.Dispose();

                // Second, schedule the disposal of the scheduler itself if applicable
                var disposableScheduler = _scheduler as IDisposable;
                if (disposableScheduler != null)
                    _scheduler.Schedule(() => disposableScheduler.Dispose());

                // Tattle to everyone else
                _activity.TrackFormat(
                    "Activity monitor with the name '{0}' of type '{1}' threw an exception while attempting to track an activity event. The monitor has been unsubscribed and will not receive any additional notifications.",
                    _underlyingMonitor.Name,
                    _underlyingMonitor.GetType().FullName
                );

                // ...and alert the other observers of what the original problem was
                _activity.TrackException(ex);
            }
        }

        public void OnError(Exception error)
        {
            try
            {
                _underlyingMonitor.OnActivityReportingError(error);
            }
            catch
            {
                /*
                 * If we get here, the IObservable<T> threw an exception and notified every subscriber of the error.
                 * We tried to handle the exception in OnActivityReportingError(), but that threw an exception too.
                 * With the OnTrack() method, the consequence of such epic failure is to dispose of the faulty
                 * monitor and shame it by telling all the other monitors about the problem. However, in this case,
                 * all of the other monitors have already been notified of the exception and furthermore cannot be
                 * notified any further problems because the event sequence has been terminated. Therefore we are
                 * out of options and are forced to eat the exception. Hopefully one of the other monitors was
                 * able to handle it successfully so that we have some chance of resolving the problem.
                 */
            }
            finally
            {
                // Event stream has terminated, so schedule our disposal
                ScheduleDisposal();
            }
        }

        public void OnCompleted()
        {
            try
            {
                _underlyingMonitor.OnTaskCompleted();
            }
            catch
            {
                /*
                 * If we get here, we end up in a similar situation compared to the OnError() delegate. Basically,
                 * we threw an exception after the task has completed and therefore we are unable to report it
                 * to anyone.
                 */
            }
            finally
            {
                // Event stream has terminated, so schedule our disposal
                ScheduleDisposal();
            }
        }

        /// <summary>
        /// Immediately releases disposable resources consumed by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                // First, dispose of our subscription
                if (_subscription != null)
                    _subscription.Dispose();

                // Third, dispose of our scheduler, if applicable
                var disposableScheduler = _scheduler as IDisposable;
                if (disposableScheduler != null)
                    disposableScheduler.Dispose();

                // Finally, make sure we don't schedule disposal more than once
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Uses the internal <see cref="IScheduler"/> to schedule the release of disposable resources consumed by this instance.
        /// </summary>
        private void ScheduleDisposal()
        {
            if (!_isDisposed)
            {
                // First, schedule the disposal of our subscription
                if (_scheduler != null && _subscription != null)
                    _scheduler.Schedule(() => _subscription.Dispose());

                // Second, schedule the disposal of the scheduler itself if applicable
                var disposableScheduler = _scheduler as IDisposable;
                if (disposableScheduler != null)
                    _scheduler.Schedule(() => disposableScheduler.Dispose());

                // Finally, make sure we don't schedule disposal more than once
                _isDisposed = true;
            }
        }
    }
}
