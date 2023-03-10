using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MarketingCampaignActivityPaged
    {
        public MarketingCampaignActivityPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? CampaignId { get; set; }
    }
}
