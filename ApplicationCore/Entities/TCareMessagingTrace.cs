using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareMessagingTrace : BaseEntity
    {
        public Guid TCareCampaignId { get; set; }
        public TCareCampaign TCareCampaign { get; set; }

        public DateTime? Sent { get; set; }

        public DateTime? Exception { get; set; }

        public DateTime? Opened { get; set; }

        public DateTime? Delivery { get; set; }

        /// <summary>
        /// psid : user facebook
        /// user_id : user zalo
        /// </summary>
        public string PSID { get; set; }

        public Guid? ChannelSocialId { get; set; }
        public FacebookPage ChannelSocial { get; set; }

        public string MessageId { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// facebook,zalo
        /// </summary>
        public string Type { get; set; }

        public string Error { get; set; }

        public Guid? TCareMessagingId { get; set; }
        public TCareMessaging TCareMessaging { get; set; }
    }
}
