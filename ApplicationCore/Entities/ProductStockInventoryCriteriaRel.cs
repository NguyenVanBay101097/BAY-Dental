using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ProductStockInventoryCriteriaRel
    {
        /// <summary>
        /// vật tư
        /// </summary>
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid StockInventoryCriteriaId { get; set; }
        public StockInventoryCriteria StockInventoryCriteria { get; set; }
    }
}
