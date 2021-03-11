using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
   public class StockInventoryLineBasic
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }

        public StockLocationSimple Location { get; set; }

        public Guid ProductId { get; set; }

        public ProductSimple Product { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }

        /// <summary>
        /// Số lượng chính thực tế
        /// </summary>
        public decimal? ProductQty { get; set; }

        /// <summary>
        /// Số lượng chính lý thuyết
        /// </summary>
        public decimal? TheoreticalQty { get; set; }

        public Guid? InventoryId { get; set; }
        public StockInventoryBasic Inventory { get; set; }

        public Guid? CompanyId { get; set; }
        public CompanySimple Company { get; set; }

        public int? Sequence { get; set; }
    }

    public class StockInventoryLineSave
    {
        public Guid Id { get; set; }

        public Guid LocationId { get; set; }

        public Guid ProductId { get; set; }

        public Guid ProductUOMId { get; set; }

        /// <summary>
        /// Số lượng chính thực tế
        /// </summary>
        public decimal? ProductQty { get; set; }

        /// <summary>
        /// Số lượng chính lý thuyết
        /// </summary>
        public decimal? TheoreticalQty { get; set; }
    }

    public class StockInventoryLineDisplay
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }

        public StockLocationSimple Location { get; set; }

        public Guid ProductId { get; set; }

        public ProductSimple Product { get; set; }

        public Guid ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }

        /// <summary>
        /// Số lượng chính thực tế
        /// </summary>
        public decimal? ProductQty { get; set; }

        /// <summary>
        /// Số lượng chính lý thuyết
        /// </summary>
        public decimal? TheoreticalQty { get; set; }

        public Guid? InventoryId { get; set; }
        public StockInventoryBasic Inventory { get; set; }

        public Guid? CompanyId { get; set; }
        public CompanySimple Company { get; set; }

        public int? Sequence { get; set; }
    }

    public class StockInventoryLinePrint
    {
        public Guid Id { get; set; }
        //public Guid LocationId { get; set; }

        //public StockLocationSimple Location { get; set; }
        public string ProductDefaultCode { get; set; }
        public string ProductName { get; set; }

        public string ProductUOMName { get; set; }

        /// <summary>
        /// Số lượng chính thực tế
        /// </summary>
        public decimal? ProductQty { get; set; }

        /// <summary>
        /// Số lượng chính lý thuyết
        /// </summary>
        public decimal? TheoreticalQty { get; set; }

        public int? Sequence { get; set; }
    }

    public class StockInventoryLineByProductId
    {
        public Guid ProductId { get; set; }
        public Guid InventoryId { get; set; }
    }
}
