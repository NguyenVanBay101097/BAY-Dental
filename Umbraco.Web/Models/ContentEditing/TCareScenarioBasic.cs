using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareScenarioBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
       

    }

    public class TCareScenarioDisplay : ToothCategoryBasic
    {
        public Guid? ChannelSocialId { get; set; }
        public FacebookPageSimple ChannelSocial { get; set; }
        public IEnumerable<TCareCampaignBasic> Campaigns { get; set; } = new List<TCareCampaignBasic>();
    }
}
