using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Hoạt động
    /// </summary>
    public class MarketingCampaignActivity: BaseEntity
    {
        public MarketingCampaignActivity()
        {
            TriggerType = "begin";
            IntervalNumber = 1;
            IntervalType = "days";
            ActivityType = "message";
        }

        public string Name { get; set; }

        public Guid CampaignId { get; set; }
        public MarketingCampaign Campaign { get; set; }

        /// <summary>
        /// Điều kiện
        /// no_sales: Không có phát sinh phiếu điều trị
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Loại hoạt động
        /// message: Nhắn tin
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

        public string TriggerType { get; set; }

        public string JobId { get; set; }

        public ICollection<MarketingTrace> Traces { get; set; } = new List<MarketingTrace>();
    }
}
