using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLinePartnerCommission : BaseEntity
    {
        /// <summary>
        /// Người được hưởng hoa hồng
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// dịch vụ
        /// </summary>
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// bảng hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }
        public Commission Commission { get; set; }

        /// <summary>
        /// phần trăm hoa hồng
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// Số tiền tính toán được hưởng hoa hồng, tính toán lại nếu price total của sale order line thay đổi
        /// </summary>
        public decimal? Amount { get; set; }
    }
}
