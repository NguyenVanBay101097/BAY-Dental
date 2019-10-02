using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockQuantMoveRel
    {
        public Guid QuantId { get; set; }
        public StockQuant Quant { get; set; }

        public Guid MoveId { get; set; }
        public StockMove Move { get; set; }
    }
}
