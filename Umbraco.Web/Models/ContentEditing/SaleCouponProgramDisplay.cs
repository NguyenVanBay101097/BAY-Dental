using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleCouponProgramDisplay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Tên chương trình
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Số tiền mua tối thiểu
        /// </summary>
        public decimal? RuleMinimumAmount { get; set; }

        public Guid? CompanyId { get; set; }

        public bool Active { get; set; }

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

        public int? CouponCount { get; set; }

        public int? OrderCount { get; set; }

        public Guid? RewardProductId { get; set; }
        public ProductSimple RewardProduct { get; set; }

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
        public string Days { get; set; }
        public IEnumerable<ProductSimple> DiscountSpecificProducts { get; set; } = new List<ProductSimple>();
        public IEnumerable<ProductCategorySimple> DiscountSpecificProductCategories { get; set; } = new List<ProductCategorySimple>();
    }

   
}
