using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderLineSave
    {
        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal Discount { get; set; }

        public Guid? CardTypeId { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }
    }
}
