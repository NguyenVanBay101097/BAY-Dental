
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

        /// <summary>
        /// Nội dung của tin nhắn gửi cho khánh hàng
        /// </summary>
        public string MessageContent { get; set; }

        /// <summary>
        /// waiting: Chờ gửi tin nhắn
        /// sent: Đã gửi
        /// exception: Gửi lỗi
        /// </summary>
        public string State { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public DateTime? Sent { get; set; }

        public DateTime? Opened { get; set; }

        public DateTime? Delivery { get; set; }

        /// <summary>
        /// Lý do exception
        /// </summary>
        public string FailureReason { get; set; }

        public string MessageId { get; set; }
    }
}

