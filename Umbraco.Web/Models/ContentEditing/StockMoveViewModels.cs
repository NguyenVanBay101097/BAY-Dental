using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class StockMoveDisplay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// mô tả cho dịch chuyển
        /// </summary>
        public string Note { get; set; }
      
        /// <summary>
        /// tên của dịch chuyển kho
        /// </summary>
        public string Name { get; set; }

        public decimal ProductUOMQty { get; set; }

        public Guid? ProductUOMId { get; set; }
        public UoMBasic ProductUOM { get; set; }
        /// <summary>
        /// sản phẩm di chuyển
        /// </summary>
        public Guid ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public int Sequence { get; set; }

        public double? PriceUnit { get; set; }
    }

    public class StockMoveOnChangeProduct
    {
        public Guid? ProductId { get; set; }
    }

    public class StockMoveOnChangeProductResult
    {
        public string Name { get; set; }
        public UoMBasic ProductUOM { get; set; }
    }

    public class StockMovePaged
    {
        public StockMovePaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public string Search { get; set; }

        public Guid? PickingId { get; set; }

    }

    public class StockMoveBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal ProductUOMQty { get; set; }
        public string ProductUOMName { get; set; }
        public int Sequence { get; set; }
    }
}
