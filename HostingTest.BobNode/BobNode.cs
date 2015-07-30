using System;
using System.Diagnostics;
using System.Threading;
using Hyper.NodeServices;
using Hyper.WcfHosting;

namespace HostingTest.BobNode
{
    class BobNode
    {
        static void Main()
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new ConsoleTraceListener());

            var container = new HyperServiceHostContainer(
                () =>
                {
                    var service = HyperNodeService.Create();
                    var host = new CancellableServiceHost(service);
                    host.RegisterCancellationDelegate(service.Cancel);

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
}