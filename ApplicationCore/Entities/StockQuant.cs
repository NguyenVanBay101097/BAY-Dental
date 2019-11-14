using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class StockQuant: BaseEntity
    {
        public StockQuant()
        {
        }

        public StockQuant(StockQuant quant)
        {
            Qty = quant.Qty;
            Cost = quant.Cost;
            LocationId = quant.LocationId;
            Location = quant.Location;
            Product = quant.Product;
            ProductId = quant.ProductId;
            InDate = quant.InDate;
            PropagatedFrom = quant.PropagatedFrom;
            PropagatedFromId = quant.PropagatedFromId;
            NegativeMove = quant.NegativeMove;
            NegativeMoveId = quant.NegativeMoveId;
            CompanyId = quant.CompanyId;
        }

        public decimal Qty { get; set; }

        public double? Cost { get; set; }

        public Guid LocationId { get; set; }

        public StockLocation Location { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        /// <summary>
        /// Incoming Date
        /// </summary>
        public DateTime? InDate { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<StockQuantMoveRel> StockQuantMoveRels { get; set; } = new List<StockQuantMoveRel>();

        /// </summary>
        public Guid? NegativeMoveId { get; set; }
        public StockMove NegativeMove { get; set; }

        /// <summary>
        /// Linked quant with negative move
        /// </summary>
        public Guid? PropagatedFromId { get; set; }
        public StockQuant PropagatedFrom { get; set; }
    }
}
