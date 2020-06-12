using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PromotionRuleDiscountCategoryItem
    {
        public Guid CategId { get; set; }
        public Guid? DiscountLineProductId { get; set; }
        public Guid? DiscountLineProductUOMId { get; set; }
    }
}
