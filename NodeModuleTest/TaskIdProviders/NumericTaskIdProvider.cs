using System;
using System.Diagnostics;
using Hyper.NodeServices.Extensibility;

namespace NodeModuleTest.TaskIdProviders
{
    public class NumericTaskIdProvider : TaskIdProviderBase, IDisposable
    {
        private static long _counter;
        private static readonly object Lock = new object();

        public override string CreateTaskId(IReadOnlyHyperNodeMessageInfo message)
        {
            if (message.CommandName == "TestLongRunningCommand")
            {
                return "LadeeDa__TestLongRunningCommandKey";
            }
            else
            {
                lock (Lock)
                {
                    return _counter++.ToString();
                }
            }
        }

        public void Dispose()
        {
            Trace.WriteLine(
                $"Disposing of {GetType().FullName}"
            );
        }
    }
}
