
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareMessage : BaseEntity
    {

        /// <summary>
        /// Thông tin của khách hàng trên fan page
        /// </summary>
        public Guid? ProfilePartnerId { get; set; }
        public FacebookUserProfile ProfilePartner { get; set; }

        /// <summary>
        /// Kênh gửi
        /// </summary>
        public Guid? ChannelSocicalId { get; set; }
        public FacebookPage ChannelSocical { get; set; }

        /// <summary>
        /// Chiến dịch
        /// </summary>
        public Guid? CampaignId { get; set; }
        public TCareCampaign Campaign { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? TCareMessagingId { get; set; }
        public TCareMessaging TCareMessaging { get; set; }

        public Guid? TCareMessagingTraceId { get; set; }
        public TCareMessagingTrace TCareMessagingTrace { get; set; }

        /// <summary>
        /// Nội dung của tin nhắn gửi cho khánh hàng
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// wating: Chờ gửi tin nhắn
        /// success: Gửi thành công
        /// fail: Gửi thất bại
        /// </summary>
        public string State { get; set; }
    }
}

