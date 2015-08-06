﻿using System;
using System.IO;
using Hyper.NodeServices.Extensibility;

namespace HyperNetExtensibilityTest.ActivityMonitors
{
    public class MondayLogFileActivityMonitor : HyperNodeServiceActivityMonitor
    {
        public static int OnNextCount;
        private static readonly object Lock = new object();

        /// <summary>
        /// Only track on Mondays
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public override bool ShouldTrack(IHyperNodeActivityEventItem activity)
        {
            return (activity.EventDateTime.DayOfWeek == DayOfWeek.Monday);
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
        {
            lock (Lock)
            {
                OnNextCount++;

                using (var writer = new StreamWriter("DatabaseWriter" + OnNextCount + ".txt", true))
                {
                    writer.WriteLine(OnNextCount);
                    writer.Flush();
                }    
            }
        }
    }
}