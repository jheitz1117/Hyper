using System;
using System.Diagnostics;
using System.Threading;
using Hyper.NodeServices;
using Hyper.WcfHosting;

namespace HostingTest.AliceNode
{
    /*
     * I really struggled with deciding whose responsibility it should be to read the app.config and
     * construct HyperNodeService instance. One crucial point I want to remind myself of later when
     * I see this and forget the train of thought is that I do not want to lay sole responsibility
     * of reading the app.config on the user's hosting application they write. If they want to do
     * so, they should be able to, and they can, but I don't want to force them to do it. By default
     * and by design, the HyperNodeServiceHostFactory class is provided precisely to read the
     * app.config and properly construct the HyperNodeService instance. Constructing one by hand
     * should be considered an advanced feature that users can take advantage of if they want
     * more control over the construction of the HyperNodeService instance.
     * 
     * The breakdown of responsibility is:
     * 1) The hosting application (console app, windows service, IIS, etc.) is in charge of:
     *     1) Creating an instance of IServiceHostFactory
     *     2) Creating an instance of IServiceHostExceptionHandler
     *     3) Creating an instance of HyperServiceHostContainer and passing in the IServiceHostFactory and
     *        IServiceHostExceptionHandler objects created in steps 1 and 2
     * 2) The IServiceHostFactory object is in charge of:
     *     1) Retrieving the hyperNet/hyperNode section from the app.config
     *     2) Creating an instance of HyperNodeService using the appropriate constructor
     *     3) Setting any public properties of the HyperNodeService
     *     4) Creating an instance of ServiceHost using the newly constructed instance of
     *        HyperNodeService
     *     
     * In other words, the hosting application shouldn't know anything nor care about what a HyperNode
     * is. The hosting application should only need to reference Hyper.WcfHosting.dll and
     * Hyper.Services.HyperNodeHosting.dll. The IServiceHostFactory implementation, on the other hand,
     * *would* need a reference to Hyper.Services.HyperNodeServices.dll so that it has everything it
     * needs to actually create the HyperNodeService and ServiceHost instances.
     * 
     */
    class AliceNode
    {
        static void Main()
        {
            Debug.Listeners.Clear();
            Debug.Listeners.Add(new ConsoleTraceListener());

            var container = new HyperServiceHostContainer(
                () =>
                {
                    var host = new CancellableServiceHost(HyperNodeService.Instance);
                    host.RegisterCancellationDelegate(HyperNodeService.Instance.Cancel);

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
