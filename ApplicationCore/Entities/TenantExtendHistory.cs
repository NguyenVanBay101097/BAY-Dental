using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Lịch sử gia hạn
    /// </summary>
    public class TenantExtendHistory : AdminBaseEntity
    {
        public TenantExtendHistory()
        {
            StartDate = DateTime.Now;
        }

        /// <summary>
        /// Ngày gia hạn
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày hết hạn
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Số chi nhánh
        /// </summary>
        public int ActiveCompaniesNbr { get; set; }

        public Guid TenantId { get; set; }
        public AppTenant AppTenant { get; set; }

        /// <summary>
        /// Không dùng nữa
        /// </summary>
        public DateTime? ApplyDate { get; set; }
    }
}
