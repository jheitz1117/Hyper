using System;
using System.Diagnostics;
using System.Threading;
using Hyper.NodeServices;
using Hyper.WcfHosting;

namespace HostingTest.AliceNode
{
    internal class AliceNode
    {
        private static void Main()
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new ConsoleTraceListener());

            var tokenSource = new CancellationTokenSource();

            var container = new HyperServiceHostContainer(
                () =>
                {
                    var host = new CancellableServiceHost(HyperNodeService.Instance);

                    // When we abort, we want to cancel the service and wait for all child tasks to complete
                    host.RegisterCancellationDelegate(
                        args =>
                        {
                            var cancelParams = (CancellationParams) args;
                            HyperNodeService.Instance.Cancel();
                            HyperNodeService.Instance.WaitAllChildTasks(cancelParams.MillisecondsTimeout, cancelParams.Token);
                        },
                        new CancellationParams
                        {
                            // TODO: Write about how these settings can be pulled from the app.config settings as a way to customize how long the service will wait after cancellation before proceeding to force a close.
                            MillisecondsTimeout = 100,
                            Timeout = TimeSpan.FromMilliseconds(2000),
                            Token = tokenSource.Token
                        }
                    );

                    return host;
                },
                new DefaultServiceHostExceptionHandler()
            );

            Console.WriteLine("Starting service...");
            if (!container.Start())
            {
                Console.WriteLine("Failed to start service. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Service started and is listening on the following addresses:");
            foreach (var endpoint in container.Endpoints)
            {
                Console.WriteLine("    " + endpoint.Address);
            }

            Console.WriteLine("Press any key to stop service...");
            Console.ReadKey();
            Console.WriteLine("Stopping service...");
            container.Stop();

            Console.WriteLine("Done.");
            Thread.Sleep(1000);
        }
    }

    internal class CancellationParams
    {
        public int MillisecondsTimeout { get; set; }
        public TimeSpan Timeout { get; set; }
        public CancellationToken Token { get; set; }
    }
}
