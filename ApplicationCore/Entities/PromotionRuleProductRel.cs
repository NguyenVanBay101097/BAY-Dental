using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PromotionRuleProductRel
    {
        public Guid RuleId { get; set; }
        public PromotionRule Rule { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? DiscountLineProductId { get; set; }
        public Product DiscountLineProduct { get; set; }
    }
}
