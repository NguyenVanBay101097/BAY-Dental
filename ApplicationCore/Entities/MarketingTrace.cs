using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MarketingTrace: BaseEntity
    {
        public Guid? ParticipantId { get; set; }
        public MarketingParticipant Participant { get; set; }

        public Guid ActivityId { get; set; }
        public MarketingCampaignActivity Activity { get; set; }

        public DateTime? ScheduleDate { get; set; }

        /// <summary>
        /// scheduled
        /// processed
        /// rejected
        /// canceled
        /// error
        /// </summary>
        public string State { get; set; }

        public string StateMsg { get; set; }
    }
}
