using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Chiến dịch marketing
    /// </summary>
    public class MarketingCampaign: BaseEntity
    {
        public MarketingCampaign()
        {
            State = "draft";
        }

        /// <summary>
        /// Tên chiến dịch
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Trạng thái. Có 3 giá trị
        /// draft: Mới
        /// running: Đang chạy
        /// stopped: Đã dừng
        /// </summary>
        public string State { get; set; }

        public DateTime? DateStart { get; set; }

        public ICollection<MarketingCampaignActivity> Activities { get; set; } = new List<MarketingCampaignActivity>();
    }
}
