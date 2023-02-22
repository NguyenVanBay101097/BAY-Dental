using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PromotionProgramSave
    {
        public string Name { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? MaximumUseNumber { get; set; }
        public IEnumerable<Guid> CompanyIds { get; set; } = new List<Guid>();
        public IEnumerable<PromotionProgramRuleSave> Rules { get; set; } = new List<PromotionProgramRuleSave>();
    }

    public class PromotionProgramRuleSave
    {
        public Guid Id { get; set; }
        public int? MinQuantity { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountFixedAmount { get; set; }
        public string DiscountApplyOn { get; set; }
        public IEnumerable<Guid> CategoryIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
    }
}
