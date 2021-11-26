using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleProductionLine : BaseEntity
    {
        public Guid SaleProductionId { get; set; }
        public SaleProduction SaleProduction { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// so luong dinh muc
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// so luong da yeu cau
        /// </summary>
        public decimal QuantityRequested { get; set; }

        public ICollection<SaleProductionLineProductRequestLineRel> ProductRequestRels { get; set; } = new List<SaleProductionLineProductRequestLineRel>();
    }
}
