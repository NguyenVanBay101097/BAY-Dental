using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountInvoicePaymentRel
    {
        public Guid PaymentId { get; set; }
        public AccountPayment Payment { get; set; }

        public Guid InvoiceId { get; set; }
        public AccountInvoice Invoice { get; set; }
    }
}
