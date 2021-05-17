using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SmsCampaign : BaseEntity
    {
        public string Name { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Gioi han tin nhan cua 1 campaign
        /// </summary>
        public int LimitMessage { get; set; }

        /// <summary>
        /// ngay bat dau campaign
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// ngay ket thuc campaign
        /// </summary>
        public DateTime? DateStart { get; set; }

        /// <summary>
        /// unlimited: vo thoi han
        /// period: Khoang thoi gian
        /// </summary>
        public string TypeDate { get; set; }

        /// <summary>
        /// running,draft,shutdown
        /// </summary>
        public string State { get; set; }
    }
}
