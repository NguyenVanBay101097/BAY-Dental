using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PromotionProgramDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? MaximumUseNumber { get; set; }
        public IEnumerable<CompanySimple> Companies { get; set; } = new List<CompanySimple>();
        public IEnumerable<PromotionProgramRuleDisplay> Rules { get; set; } = new List<PromotionProgramRuleDisplay>();
        public int? OrderCount { get; set; }
    }

    public class PromotionProgramRuleDisplay
    {
        public Guid Id { get; set; }
        public decimal? MinQuantity { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountFixedAmount { get; set; }
        public string DiscountApplyOn { get; set; }
        public IEnumerable<ProductCategoryBasic> Categories { get; set; } = new List<ProductCategoryBasic>();
        public IEnumerable<ProductSimple> Products { get; set; } = new List<ProductSimple>();
    }
}
