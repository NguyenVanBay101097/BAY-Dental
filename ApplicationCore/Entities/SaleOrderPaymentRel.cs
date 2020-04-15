using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderPaymentRel
    {
        public Guid SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        public Guid PaymentId { get; set; }
        public AccountPayment Payment { get; set; }
    }
}
