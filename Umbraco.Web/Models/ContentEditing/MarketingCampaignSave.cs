using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MarketingCampaignSave
    {
        public string Name { get; set; }
        public IEnumerable<MarketingCampaignActivitySave> Activities { get; set; } = new List<MarketingCampaignActivitySave>();
    }
}
