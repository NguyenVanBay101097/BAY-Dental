using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MarketingParticipant: BaseEntity
    {
        public Guid CampaignId { get; set; }
        public MarketingCampaign Campaign { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public string State { get; set; }
    }
}
