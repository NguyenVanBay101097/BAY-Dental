using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class TCareScenario : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<TCareCampaign> Campaigns { get; set; } = new List<TCareCampaign>();

        // Kênh gửi
        public Guid? ChannelSocialId { get; set; }
        public FacebookPage ChannelSocial { get; set; }
        public string ChannalType { get; set; }

        /// <summary>
        /// auto_everyday: Chạy tự động hàng ngày
        /// auto_custom: Tùy chỉnh chạy tự động
        /// manual: Chạy thủ công
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// custom1: Ngày giờ cụ thể hàng năm ví dụ ngày 8/3 lúc 8h30
        /// custom2: Ngày trong tháng ví dụ ngày 3 mỗi tháng lúc 8h30
        /// </summary>
        public string AutoCustomType { get; set; }

        public int? CustomDay { get; set; }

        public int? CustomMonth { get; set; }

        public int? CustomHour { get; set; }

        public int? CustomMinute { get; set; }

        /// <summary>
        /// định dạng: {db}-tcare-scenario-{id}-custom
        /// </summary>
        public string JobId { get; set; }
    }
}
