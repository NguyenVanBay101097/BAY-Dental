using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrderLineInvoiceRel
    {
        public Guid OrderLineId { get; set; }
        public ServiceCardOrderLine OrderLine { get; set; }

        public Guid InvoiceLineId { get; set; }
        public AccountMoveLine InvoiceLine { get; set; }
    }
}
