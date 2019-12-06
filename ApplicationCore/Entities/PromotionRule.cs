using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Điều kiện khuyến mãi và phần thưởng
    /// </summary>
    public class PromotionRule: BaseEntity
    {
        public Guid ProgramId { get; set; }
        public PromotionProgram Program { get; set; }

        /// <summary>
        /// Số lượng mua tối thiểu
        /// </summary>
        public decimal? MinQuantity { get; set; }

        /// <summary>
        /// Loại chiết khấu
        /// percentage: phần trăm, fixed_amount: tiền cố định
        /// </summary>
        public string DiscountType { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountFixedAmount { get; set; }

        /// <summary>
        /// 3_global: Chiết khấu trên tất cả dịch vụ
        /// 2_product_category: Chiết khấu trên vài nhóm dịch vụ
        /// 0_product_variant: Chiết khấu trên vài dịch vụ
        /// </summary>
        public string DiscountApplyOn { get; set; }

        public ICollection<PromotionRuleProductCategoryRel> RuleCategoryRels { get; set; } = new List<PromotionRuleProductCategoryRel>();

        public ICollection<PromotionRuleProductRel> RuleProductRels { get; set; } = new List<PromotionRuleProductRel>();

        /// <summary>
        /// Sản phẩm cho chiết khấu toàn bộ đơn hàng
        /// </summary>
        public Guid? DiscountLineProductId { get; set; }
        public Product DiscountLineProduct { get; set; }
    }
}
