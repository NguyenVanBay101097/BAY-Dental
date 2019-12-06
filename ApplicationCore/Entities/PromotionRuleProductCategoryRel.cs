using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PromotionRuleProductCategoryRel
    {
        public Guid RuleId { get; set; }
        public PromotionRule Rule { get; set; }

        public Guid CategId { get; set; }
        public ProductCategory Categ { get; set; }

        public Guid? DiscountLineProductId { get; set; }
        public Product DiscountLineProduct { get; set; }
    }
}
