using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrderPaymentRel
    {
        public Guid CardOrderId { get; set; }
        public ServiceCardOrder CardOrder { get; set; }

        public Guid PaymentId { get; set; }
        public AccountPayment Payment { get; set; }
    }
}
