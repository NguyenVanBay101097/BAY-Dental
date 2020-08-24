using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLinePaymentRel : BaseEntity
    {
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        public Guid PaymentId { get; set; }
        public AccountPayment Payment { get; set; }

        /// <summary>
        /// số tiền thanh toán
        /// </summary>
        public decimal? AmountPrepaid { get; set; }
    }
}
