using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareConfig : BaseEntity
    {
        public TCareConfig()
        {
            JobCampaignHour = 0;
            JobCampaignMinute = 0;
            JobMessagingMinute = 60;
            JobMessageMinute = 60;
        }
        /// <summary>
        /// 0->23
        /// </summary>
        public int? JobCampaignHour { get; set; }
        /// <summary>
        /// 0->59
        /// </summary>
        public int? JobCampaignMinute { get; set; }
        /// <summary>
        /// >0
        /// </summary>
        public int? JobMessagingMinute { get; set; }
        /// <summary>
        /// >0
        /// </summary>
        public int? JobMessageMinute { get; set; }
    }
}
