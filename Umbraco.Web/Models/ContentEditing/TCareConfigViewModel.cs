using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class TCareConfigSave
    {
        public int? JobCampaignHour { get; set; }
        public int? JobCampaignMinute { get; set; }
        public int? JobMessagingMinute { get; set; }
        public int? JobMessageMinute { get; set; }
    }
}
