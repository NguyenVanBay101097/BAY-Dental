using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class CommissionSettlement : BaseEntity
    {
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// số tiền thanh toán
        /// </summary>
        public decimal? BaseAmount { get; set; }

        /// <summary>
        /// phần trăm hoa hồng
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// tiền hoa hồng bác sĩ
        /// </summary>
        public decimal? Amount { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        public Guid? PaymentId { get; set; }
        public AccountPayment Payment { get; set; }
    }
}
