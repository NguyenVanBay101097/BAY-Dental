using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCoupon: BaseEntity
    {
        public SaleCoupon()
        {
            State = "new";
        }

        public string Code { get; set; }
        public Guid? ProgramId { get; set; }
        public SaleCouponProgram Program { get; set; }
        /// <summary>
        /// Trạng thái
        /// reserved: Để dành riêng, new: Còn giá trị, used: Đã sử dụng, expired: Hết hiệu lực
        public string State { get; set; }
        public DateTime? DateExpired { get; set; }

        /// <summary>
        /// Đơn hàng được áp dụng
        /// </summary>
        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// reserve order
        /// </summary>
        public Guid? OrderId { get; set; }
        public SaleOrder Order { get; set; }
    }
}
