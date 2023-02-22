using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockInventoryLine : BaseEntity
    {
        public StockInventoryLine()
        {
            ProductQty = 0;
            TheoreticalQty = 0;
        }

        public Guid LocationId { get; set; }

        public StockLocation Location { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoM ProductUOM { get; set; }

        /// <summary>
        /// Số lượng chính thực tế
        /// </summary>
        public decimal? ProductQty { get; set; }

        /// <summary>
        /// Số lượng chính lý thuyết
        /// </summary>
        public decimal? TheoreticalQty { get; set; }

        public Guid? InventoryId { get; set; }
        public StockInventory Inventory { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? Sequence { get; set; }
    }
}
