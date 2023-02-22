using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLineToothRel
    {
        public Guid SaleLineId { get; set; }
        public SaleOrderLine SaleLine { get; set; }

        public Guid ToothId { get; set; }
        public Tooth Tooth { get; set; }
    }
}
