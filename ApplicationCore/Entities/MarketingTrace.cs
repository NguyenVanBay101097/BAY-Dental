using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    //trace chỗ mỗi tin nhắn gửi
    public class MarketingTrace: BaseEntity
    {
        //public Guid? ParticipantId { get; set; }
        //public MarketingParticipant Participant { get; set; }

        public Guid ActivityId { get; set; }
        public MarketingCampaignActivity Activity { get; set; }

        public DateTime? Sent { get; set; }

        public DateTime? Exception { get; set; }

        public DateTime? Read { get; set; }

        public DateTime? Delivery { get; set; }

        public string MessageId { get; set; }

        public Guid? UserProfileId { get; set; }
        public FacebookUserProfile UserProfile { get; set; }

      

        //public string State { get; set; }

        //public string StateMsg { get; set; }
    }
}
