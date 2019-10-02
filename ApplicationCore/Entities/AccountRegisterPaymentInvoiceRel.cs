using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountRegisterPaymentInvoiceRel
    {
        public Guid PaymentId { get; set; }
        public AccountRegisterPayment Payment { get; set; }
        public Guid InvoiceId { get; set; }
        public AccountInvoice Invoice { get; set; }
    }
}
