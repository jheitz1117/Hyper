using System;
using System.IO;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.ActivityMonitors
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
        public override bool ShouldTrack(IHyperNodeActivityEventItem activity)
        {
            return (activity.EventDateTime.DayOfWeek == DayOfWeek.Monday);
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
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
