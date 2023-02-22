using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PurchaseOrderLineSave
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Sequence { get; set; }
        public decimal ProductQty { get; set; }
        public Guid? ProductUOMId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? PriceSubtotal { get; set; }
        public decimal? PriceTotal { get; set; }
        public decimal? PriceTax { get; set; }
        public decimal PriceUnit { get; set; }
        public string State { get; set; }
        public decimal? Discount { get; set; }
    }
}
