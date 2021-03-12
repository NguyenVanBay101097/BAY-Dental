using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockInventoryCriteria: BaseEntity
    {
        public string Name { get; set; }
        public string Note { get; set; }
    }
}
