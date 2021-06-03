using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponProgramSave
    {
        /// <summary>
        /// Tên chương trình
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Số tiền mua tối thiểu
        /// </summary>
        public decimal? RuleMinimumAmount { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// Loại chiết khấu
        /// percentage: phần trăm, fixed_amount: tiền cố định
        /// </summary>
        public string DiscountType { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountFixedAmount { get; set; }

        /// <summary>
        /// Thời gian hiệu lực khi coupon đc tạo ra: ngày
        /// </summary>
        public int? ValidityDuration { get; set; }

        public string ProgramType { get; set; }

        public Guid? RewardProductId { get; set; }

        public string RewardType { get; set; }

        public int? MaximumUseNumber { get; set; }

        public int? RuleMinQuantity { get; set; }

        public string PromoCodeUsage { get; set; }

        public string PromoCode { get; set; }

        public string PromoApplicability { get; set; }

        public decimal? DiscountMaxAmount { get; set; }

        public int? RewardProductQuantity { get; set; }

        public DateTime? RuleDateFrom { get; set; }

        public DateTime? RuleDateTo { get; set; }

        public string DiscountApplyOn { get; set; }
        public bool? NotIncremental { get; set; }

        public bool IsApplyDayOfWeek { get; set; }

        public string ApplyPartnerOn { get; set; }

        public bool IsApplyMinimumDiscount { get; set; }

        public bool IsApplyMaxDiscount { get; set; }

        public IEnumerable<string> Days { get; set; } = new List<string>();
        public IEnumerable<Guid> DiscountSpecificProductIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> DiscountSpecificProductCategoryIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> DiscountSpecificPartnerIds { get; set; } = new List<Guid>();
        public IEnumerable<Guid> DiscountMemberLevelIds { get; set; } = new List<Guid>();
    }
}
