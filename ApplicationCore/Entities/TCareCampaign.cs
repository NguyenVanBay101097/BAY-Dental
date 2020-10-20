using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Kich bản chăm sóc
    /// </summary>
    public class TCareCampaign: BaseEntity
    {
        public TCareCampaign()
        {
            Active = false;
            SheduleStart = DateTime.Today;
        }

        public string Name { get; set; }

        /// <summary>
        /// File xml convert ra các node ở giao diện
        /// </summary>
        public string GraphXml { get; set; }

        /// <summary>
        /// Thời gian chạy (DateTime)
        /// </summary>
        public DateTime? SheduleStart { get; set; }

        /// <summary>
        /// Lịch sử chạy của 1 message (để sau khi gán tag có thể tìm lại đúng user đó) 
        /// </summary>

        /// <summary>
        /// draft : mới 
        /// running : đang chạy
        /// stopped : dừng
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Lưu lại Id của hangfirer
        /// </summary>
        public string RecurringJobId { get; set; }

        public Guid? TCareScenarioId { get; set; }
        public TCareScenario TCareScenario { get; set; }

        /// <summary>
        /// true: Đang chạy
        /// false: nháp hoặc đã dừng
        /// </summary> 
        public bool Active { get; set; }

        public Guid? TagId { get; set; }
        public PartnerCategory Tag { get; set; }

        public Guid? FacebookPageId { get; set; }
        public FacebookPage FacebookPage { get; set; }
    }
}
