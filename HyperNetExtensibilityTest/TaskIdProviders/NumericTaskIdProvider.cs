using System;
using System.Diagnostics;
using Hyper.NodeServices.Extensibility;

namespace HyperNetExtensibilityTest.TaskIdProviders
{
    public class NumericTaskIdProvider : TaskIdProviderBase, IDisposable
    {
        private static long _counter;
        private static readonly object Lock = new object();

        public override string CreateTaskId(IHyperNodeMessageContext context)
        {
            lock (Lock)
            {
                return (_counter++).ToString();
            }    
        }

        public void Dispose()
        {
            Trace.WriteLine(
                string.Format(
                    "Disposing of {0}",
                    GetType().FullName
                )
            );
        }
    }
}
