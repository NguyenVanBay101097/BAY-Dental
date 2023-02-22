using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLineInvoiceRel
    {
        public Guid OrderLineId { get; set; }
        public SaleOrderLine OrderLine { get; set; }

        public Guid InvoiceLineId { get; set; }
        public AccountInvoiceLine InvoiceLine { get; set; }
    }
}
