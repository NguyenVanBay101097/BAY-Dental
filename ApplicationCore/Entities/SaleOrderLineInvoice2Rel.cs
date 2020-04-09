using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLineInvoice2Rel
    {
        public Guid OrderLineId { get; set; }
        public SaleOrderLine OrderLine { get; set; }

        public Guid InvoiceLineId { get; set; }
        public AccountMoveLine InvoiceLine { get; set; }
    }
}
