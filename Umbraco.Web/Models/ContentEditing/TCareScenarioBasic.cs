﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareScenarioBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
       

    }

    public class TCareScenarioDisplay 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid? ChannelSocialId { get; set; }
        public FacebookPageSimple ChannelSocial { get; set; }
        public string Type { get; set; }

        public string AutoCustomType { get; set; }

        public int? CustomDay { get; set; }

        public int? CustomMonth { get; set; }

        public int? CustomHour { get; set; }

        public int? CustomMinute { get; set; }

        public IEnumerable<TCareCampaignBasic> Campaigns { get; set; } = new List<TCareCampaignBasic>();
    }
}
