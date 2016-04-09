using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Describes an activity event reported by a <see cref="IHyperNodeService"/>.
    /// </summary>
    [DataContract]
    public class HyperNodeActivityItem
    {
        /// <summary>
        /// The date and time the event happened.
        /// </summary>
        [DataMember]
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// The name of the agent reporting the activity event.
        /// </summary>
        [DataMember]
        public string Agent { get; set; }

        /// <summary>
        /// The amount of time that has elapsed since the first <see cref="HyperNodeActivityItem"/> was tracked for the task.
        /// This value may be null unless diagnostics are enabled.
        /// </summary>
        [DataMember]
        public TimeSpan? Elapsed { get; set; }

        /// <summary>
        /// A description of the activity event.
        /// </summary>
        [DataMember]
        public string EventDescription { get; set; }

        /// <summary>
        /// A longer, more detailed description of the activity event.
        /// </summary>
        [DataMember]
        public string EventDetail { get; set; }

        /// <summary>
        /// A numeric value representing the progress of a command module. This value may be used in conjunction
        /// with the <see cref="ProgressTotal"/> property to obtain a percentile.
        /// </summary>
        [DataMember]
        public double? ProgressPart { get; set; }

        /// <summary>
        /// A numeric value representing the progress total of a command module. This value may be used in conjunction
        /// with the <see cref="ProgressPart"/> property to obtain a percentile.
        /// </summary>
        [DataMember]
        public double? ProgressTotal { get; set; }

        /// <summary>
        /// Calculates the progress of the task as a percentile by dividing the <see cref="ProgressPart"/> property by the <see cref="ProgressTotal"/> property.
        /// This operation is arithmetically safe.
        /// </summary>
        public double? ProgressPercentage
        {
            get
            {
                if (ProgressPart.HasValue && ProgressTotal.HasValue && ProgressTotal != 0)
                    return ProgressPart.Value / ProgressTotal.Value;

                return null;
            }
        }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeActivityItem"/>.
        /// </summary>
        public HyperNodeActivityItem()
        {
            EventDateTime = DateTime.Now;
        }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeActivityItem"/> using the specified agent.
        /// </summary>
        /// <param name="agent">The agent creating this instance.</param>
        public HyperNodeActivityItem(string agent)
            : this()
        {
            Agent = agent;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HyperNodeActivityItem"/> that is a deep copy of the specified instance.
        /// </summary>
        /// <param name="originalItem">The original instance to copy.</param>
        public HyperNodeActivityItem(HyperNodeActivityItem originalItem)
        {
            if (originalItem == null)
                throw new ArgumentNullException(nameof(originalItem));

            Agent = originalItem.Agent;
            Elapsed = originalItem.Elapsed;
            EventDateTime = originalItem.EventDateTime;
            EventDescription = originalItem.EventDescription;
            EventDetail = originalItem.EventDetail;
            ProgressPart = originalItem.ProgressPart;
            ProgressTotal = originalItem.ProgressTotal;
        }
    }
}
