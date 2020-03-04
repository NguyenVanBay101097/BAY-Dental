using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class MarketingCampaignActivityDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Điều kiện
        /// no_sales: Không có phát sinh phiếu điều trị
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Loại hoạt động. Xác định sẽ gửi tin nhắn thông qua kênh nào zalo hay facebook...
        /// zalo: Zalo
        /// facebook: Facebook
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// Nội dung
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// "hours", "days", "weeks", "months"
        /// </summary>
        public string IntervalType { get; set; }

        public int? IntervalNumber { get; set; }

        public int? Sequence { get; set; }
    }
}
