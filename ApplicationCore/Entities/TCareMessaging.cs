using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareMessaging : BaseEntity
    {
        /// <summary>
        /// Ngày lên lịch gửi
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Nội dung tổng quát gửi cho khách hàng
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Số lượng khách hàng được gửi trong ngày 
        /// </summary>
        public int CountPartner { get; set; }

        //--Kenh gui ---//

        /// <summary>
        ///  priority : ưu tiên
        ///  fixed : cố định
        /// </summary>
        //public string ChannelType { get; set; }

        public ICollection<TCareMessagingTrace> TCareMessagingTraces { get; set; } = new List<TCareMessagingTrace>();
        public ICollection<TCareMessage> TCareMessages { get; set; } = new List<TCareMessage>();

        public Guid TCareCampaignId { get; set; }
        public TCareCampaign TCareCampaign { get; set; }

        public Guid? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }

        /// <summary>
        /// in_queue: Chờ gửi
        /// done: Hoàn thành
        /// </summary>
        public string State { get; set; }
    }
}
