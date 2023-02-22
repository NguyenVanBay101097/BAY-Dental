using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PromotionRuleDiscountProductItem
    {
        public Guid ProductId { get; set; }
        public Guid? DiscountLineProductId { get; set; }
        public Guid? DiscountLineProductUOMId { get; set; }
    }
}
