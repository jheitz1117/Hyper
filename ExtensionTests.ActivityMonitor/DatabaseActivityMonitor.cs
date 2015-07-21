using System;
using System.IO;
using Hyper.Services.HyperNodeActivityTracking;
using Hyper.Services.HyperNodeExtensibility;

namespace ExtensionTests.ActivityMonitor
{
    public class DatabaseActivityMonitor : HyperNodeServiceActivityMonitor
    {
        public static int OnNextCount;
        private readonly object _lock = new object();

        /// <summary>
        /// Only track on Tuesdays
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public override bool ShouldTrack(HyperNodeActivityEventItem activity)
        {
            return (activity.EventDateTime.DayOfWeek == DayOfWeek.Monday);
        }

        public override void OnNext(HyperNodeActivityEventItem activity)
        {
            OnNextCount++;

            lock (_lock)
            {
                using (var writer = new StreamWriter("DatabaseWriter.txt", true))
                {
                    writer.WriteLine(OnNextCount);
                    writer.Flush();
                }    
            }
        }
    }
}
