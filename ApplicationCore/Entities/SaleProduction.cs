using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleProduction : BaseEntity
    { 
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// so luong dich vu
        /// </summary>
        public decimal Quantity { get; set; }

        public ICollection<SaleProductionLine> Lines { get; set; } = new List<SaleProductionLine>();

        public Guid? CompanyId { get; set; }

        public Company Company { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public ICollection<SaleOrderLineSaleProductionRel> SaleOrderLineRels { get; set; } = new List<SaleOrderLineSaleProductionRel>();
    }
}
