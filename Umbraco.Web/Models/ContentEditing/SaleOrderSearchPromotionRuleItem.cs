using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderSearchPromotionRuleItem
    {
        public Guid Id { get; set; }
        public string DiscountApplyOn { get; set; }
        public IEnumerable<PromotionRuleDiscountProductItem> DiscountProductItems { get; set; }
        public IEnumerable<PromotionRuleDiscountCategoryItem> DiscountCategoryItems { get; set; }
        public decimal MinQuantity { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountFixedAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string ProgramName { get; set; }
        public Guid ProgramId { get; set; }
        public Guid? DiscountLineProductId { get; set; }
        public Guid? DiscountLineProductUOMId { get; set; }
    }
}
