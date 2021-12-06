using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleProductionLineProductRequestLineRel
    {
        public Guid SaleProductionLineId { get; set; }
        public SaleProductionLine SaleProductionLine { get; set; }

        public Guid ProductRequestLineId { get; set; }
        public ProductRequestLine ProductRequestLine { get; set; }

    }
}
