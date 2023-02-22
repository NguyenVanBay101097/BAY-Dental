using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderPaymentAccountPaymentRel
    {
        public Guid SaleOrderPaymentId { get; set; }
        public SaleOrderPayment SaleOrderPayment { get; set; }

        public Guid PaymentId { get; set; }
        public AccountPayment Payment { get; set; }
    }
}
