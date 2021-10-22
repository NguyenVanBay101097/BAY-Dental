using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class StockPickingPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }

        public PartnerPrintTemplate Partner { get; set; }

        public string Name { get; set; }

        public DateTime? DateCreated { get; set; }

        public IEnumerable<StockMovePrintTemplate> MoveLines { get; set; } = new List<StockMovePrintTemplate>();

        public string Note { get; set; }

        public string State { get; set; }

        public string CreatedByName { get; set; }
    }

    public class StockMovePrintTemplate
    {
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
        public UoMBasicPrintTemplate ProductUOM { get; set; }
        /// <summary>
        /// sản phẩm di chuyển
        /// </summary>
        public Guid ProductId { get; set; }
        public ProductSimplePrintTemplate Product { get; set; }

        public int Sequence { get; set; }

        public double? PriceUnit { get; set; }
    }
}
