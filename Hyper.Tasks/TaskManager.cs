using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hyper.Tasks
{
    public class TaskManager
    {
        private readonly List<Task> _tasks = new List<Task>();
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        bool _isRunning;

        /// <summary>
        /// List of tasks to manage in this instance of TaskManager
        /// </summary>
        public List<IManagedTask> ManagedTasks
        {
            get { return _managedTasks; }
            set
            {
                _managedTasks = value ?? new List<IManagedTask>();
            }
        } private List<IManagedTask> _managedTasks = new List<IManagedTask>();

        /// <summary>
        /// Starts all of the managed tasks collected so far in child threads. If the list is empty, this method does nothing.
        /// </summary>
        public async void Start()
        {
            if (this.ManagedTasks.Count == 0) return;
            if (_isRunning)
            { throw new InvalidOperationException("You may not call the Start() method while there are still tasks running."); }

            _isRunning = true;

            // Reset our list of tasks
            _tasks.Clear();

            foreach (var task in this.ManagedTasks)
            {
                var loopingTask = task as LoopingManagedTaskBase;
                if (loopingTask != null)
                {
                    StartNewLoopingTask(loopingTask.ExecuteTask, loopingTask.Delay, loopingTask.ExceptionHandler);
                }
                else
                {
                    StartNewTask(task.ExecuteTask, task.ExceptionHandler);
                }
            }

            // Wait for all the tasks to complete
            await Task.WhenAll(_tasks);

            // Reset
            _isRunning = false;
        }

        /// <summary>
        /// Sends the stop signal for all of the managed tasks to halt their execution.
        /// </summary>
        public void Stop()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Sends the stop signal for all of the managed tasks to halt their execution and blocks the calling thread until the tasks complete.
        /// </summary>
        public void BlockingStop()
        {
            Stop();
            Task.WaitAll(_tasks.ToArray());
        }

        /// <summary>
        /// Sends the stop signal for all of the managed tasks to halt their execution and blocks the calling thread until the tasks complete.
        /// </summary>
        /// <param name="timeout">A System.TimeSpan that represents the number of milliseconds to wait, or a System.TimeSpan that reprsents -1 milliseconds to wait indefinitely.</param>
        public void BlockingStop(TimeSpan timeout)
        {
            Stop();
            Task.WaitAll(_tasks.ToArray(), timeout);
        }

        /// <summary>
        /// Sends the stop signal for all of the managed tasks to halt their execution and blocks the calling thread until the tasks complete.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or System.Threading.Timeout.Infinite (-1) to wait indefinitely.</param>
        public void BlockingStop(int millisecondsTimeout)
        {
            Stop();
            Task.WaitAll(_tasks.ToArray(), millisecondsTimeout);
        }

        /// <summary>
        /// Starts a new thread which executes a one-time action with a custom exception handler.
        /// </summary>
        /// <param name="taskMethod">An Action representing the method to perform.</param>
        /// <param name="exceptionHandler">IManagedTaskExceptionHandler used to handle exceptions for the new task</param>
        private void StartNewTask(Action<CancellationToken> taskMethod, IManagedTaskExceptionHandler exceptionHandler)
        {
            if (taskMethod == null) { throw new ArgumentNullException("taskMethod"); }

            _tasks.Add(Task.Factory.StartNew(() =>
            {
                try
                {
                    taskMethod(_cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                    {
                        exceptionHandler.HandleException(ex);
                    }
                }
            }));
        }

        /// <summary>
        /// Starts a new thread which executes a recurring action with a custom exception handler.
        /// </summary>
        /// <param name="taskMethod">An Action representing the method to perform.</param>
        /// <param name="delay">a System.TimeSpan indicating how long to wait after the task method completes before restarting it.</param>
        /// <param name="exceptionHandler">IManagedTaskExceptionHandler used to handle exceptions for the new task</param>
        private void StartNewLoopingTask(Action<CancellationToken> taskMethod, TimeSpan delay, IManagedTaskExceptionHandler exceptionHandler)
        {
            if (taskMethod == null) { throw new ArgumentNullException("taskMethod"); }
            if (delay == null) { throw new ArgumentNullException("delay"); }
            if (delay.CompareTo(TimeSpan.Zero) == delay.CompareTo(TimeSpan.MaxValue)) { throw new ArgumentOutOfRangeException("delay"); }

            _tasks.Add(Task.Factory.StartNew(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        taskMethod(_cancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        if (exceptionHandler != null)
                        {
                            exceptionHandler.HandleException(ex);
                        }
                    }

                    await Task.Delay(delay, _cancellationTokenSource.Token);
                }
            }));
        }
    }
}
