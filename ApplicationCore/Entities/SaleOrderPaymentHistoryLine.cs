using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderPaymentHistoryLine : BaseEntity
    {
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        public Guid SaleOrderPaymentId { get; set; }
        public SaleOrderPayment SaleOrderPayment { get; set; }

        public decimal Amount { get; set; }
    }
}
