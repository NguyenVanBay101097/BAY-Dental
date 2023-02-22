using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class TCareMessagingBasic
    {
        public Guid Id { get; set; }   

        public string ScenarioName { get; set; }

        public string CampaignName { get; set; }

        public DateTime? ScheduleDate { get; set; }

        /// <summary>
        /// tổng số người nhận tin
        /// </summary>
        public int PartnerTotal { get; set; }
        /// <summary>
        /// tổng tin nhắn
        /// </summary>
        public int MessageTotal { get; set; }
        /// <summary>
        /// tổng tin nhắn đã gửi
        /// </summary>
        public int MessageSentTotal { get; set; }
        /// <summary>
        /// tổng tin nhắn gửi lỗi
        /// </summary>
        public int MessageExceptionTotal { get; set; }
        /// <summary>
        /// draft: Mới
        /// in_queue: Chờ gửi
        /// sending: Đang gửi
        /// done: Hoàn thành
        /// </summary>
        public string State { get; set; }
    }
}
