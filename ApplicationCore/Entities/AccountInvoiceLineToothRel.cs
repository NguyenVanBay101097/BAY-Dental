using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountInvoiceLineToothRel
    {
        public Guid InvoiceLineId { get; set; }
        public AccountInvoiceLine InvoiceLine { get; set; }

        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
