using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLineSaleProductionRel
    {
        public Guid OrderLineId { get; set; }
        public SaleOrderLine OrderLine { get; set; }
        public Guid SaleProductionId { get; set; }
        public SaleProduction SaleProduction { get; set; }
    }
}
