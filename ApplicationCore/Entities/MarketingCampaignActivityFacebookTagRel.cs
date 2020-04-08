using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class MarketingCampaignActivityFacebookTagRel
    {
        public Guid ActivityId { get; set; }
        public MarketingCampaignActivity Activity { get; set; }

        public Guid TagId { get; set; }
        public FacebookTag Tag { get; set; }
    }
}
