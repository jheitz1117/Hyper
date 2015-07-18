using System;
using System.Diagnostics;
using System.Threading;
using Hyper.Services.HyperNodeHosting;
using Hyper.WcfHosting;

namespace HostingTest.BobNode
{
    class BobNode
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new ConsoleTraceListener());

            var host = new HyperServiceHost(new HyperNodeServiceHostFactory(), new DefaultServiceHostExceptionHandler());

            Console.WriteLine("Starting service...");
            if (!host.Start())
            {
                Console.WriteLine("Failed to start service. Press any key to continue...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Service started and is listening on the following addresses:");
            foreach (var endpoint in host.Endpoints)
            {
                Console.WriteLine("    " + endpoint.Address);
            }

            Console.WriteLine("Press any key to stop service...");
            Console.ReadKey();
            Console.WriteLine("Stopping service...");
            host.Stop();

            Console.WriteLine("Done.");
            Thread.Sleep(1000);
        }
    }
}