using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareMessaging : BaseEntity
    {
        public TCareMessaging()
        {
            MessagingModel = "partner";
            State = "draft";
        }

        /// <summary>
        /// Ngày lên lịch gửi
        /// </summary>
        public DateTime? ScheduleDate { get; set; }

        /// <summary>
        /// Nội dung tổng quát gửi cho khách hàng
        /// </summary>
        public string Content { get; set; }

        public ICollection<TCareMessage> TCareMessages { get; set; } = new List<TCareMessage>();

        public Guid TCareCampaignId { get; set; }
        public TCareCampaign TCareCampaign { get; set; }

        public Guid? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }

        /// <summary>
        /// draft: Mới
        /// in_queue: Chờ gửi
        /// sending: Đang gửi
        /// exception: that bai
        /// done: Hoàn thành
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Đối tượng nhận tin
        /// partner: Partner
        /// </summary>
        public string MessagingModel { get; set; }

        public ICollection<TCareMessagingPartnerRel> PartnerRecipients { get; set; } = new List<TCareMessagingPartnerRel>();

        /// <summary>
        /// một tin nhắn có thể có 1 coupon
        /// </summary>
        public Guid? CouponProgramId { get; set; }
        public SaleCouponProgram CouponProgram { get; set; }
    }
}
