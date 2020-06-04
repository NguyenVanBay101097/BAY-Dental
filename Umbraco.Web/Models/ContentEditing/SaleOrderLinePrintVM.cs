using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLinePrintVM
    {
        public string ProductName { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        public int? Sequence { get; set; }

        public string DiscountType { get; set; }

        public decimal Discount { get; set; }

        public decimal? DiscountFixed { get; set; }
    }
}
