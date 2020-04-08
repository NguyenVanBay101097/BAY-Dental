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
        /// action: Hành động
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

        /// <summary>
        /// begin: beginning of campaign
        /// act: another activity
        /// message_open: Message opened
        /// </summary>
        public string TriggerType { get; set; }

        public string JobId { get; set; }

        public ICollection<MarketingTrace> Traces { get; set; } = new List<MarketingTrace>();

        public Guid? MessageId { get; set; }
        public MarketingMessage Message { get; set; }

        /// <summary>
        /// add_tags: Thêm tags
        /// remove_tags: Gỡ tags
        /// </summary>
        public string ActionType { get; set; }

        public ICollection<MarketingCampaignActivityFacebookTagRel> TagRels { get; set; } = new List<MarketingCampaignActivityFacebookTagRel>();
    }
}
