using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderLineDisplay
    {
        public Guid Id { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal Discount { get; set; }

        public Guid? CardTypeId { get; set; }
        public ServiceCardTypeBasic CardType { get; set; }

        public int? Sequence { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        public int CardCount { get; set; }
    }
}
