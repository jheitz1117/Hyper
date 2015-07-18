using System;

namespace Hyper.Tasks
{
    public abstract class LoopingManagedTaskBase : ManagedTaskBase
    {
        /// <summary>
        /// Specifies how long to wait after this task finishes running before starting it again.
        /// </summary>
        public TimeSpan Delay { get; set; }
    }
}
