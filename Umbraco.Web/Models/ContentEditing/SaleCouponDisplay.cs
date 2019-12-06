using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponDisplay
    {
        public Guid Id { get; set; }

        public string Code { get; set; }
        public Guid? ProgramId { get; set; }
        public string ProgramName { get; set; }
        /// <summary>
        /// Trạng thái
        /// reserved: Để dành riêng, new: Còn giá trị, used: Đã sử dụng, expired: Hết hiệu lực
        public string State { get; set; }
        public DateTime? DateExpired { get; set; }
        public Guid? PartnerId { get; set; }
        public string PartnerName { get; set; }

        /// <summary>
        /// reserve order
        /// </summary>
        public Guid? OrderId { get; set; }
        public string OrderName { get; set; }

        public Guid? SaleOrderId { get; set; }
        public string SaleOrderName { get; set; }
    }
}
