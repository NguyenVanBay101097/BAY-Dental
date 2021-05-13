using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgram: BaseEntity
    {
        public SaleCouponProgram()
        {
            Active = false;
            RewardType = "discount";
            PromoApplicability = "on_current_order";
            ProgramType = "coupon_program";
            RewardProductQuantity = 1;
            RuleMinQuantity = 1;
            RuleMinimumAmount = 0;
            DiscountMaxAmount = 0;
            DiscountApplyOn = "on_order";
        }

        /// <summary>
        /// Tên chương trình
        /// </summary>
        public string Name { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// waiting: Chưa chạy
        /// running: Đang chạy
        /// paused: Tạm ngừng
        /// expired: Hết hạn
        /// </summary>
        //public string Status { get; set; }

        public bool IsPaused { get; set; }

        /// <summary>
        /// Coupon program will be applied based on given sequence if multiple programs are
        /// defined on same condition(For minimum amount)
        /// </summary>
        public int? Sequence { get; set; }

        public int? MaximumUseNumber { get; set; }

        /// <summary>
        /// Số tiền mua tối thiểu
        /// </summary>
        public decimal? RuleMinimumAmount { get; set; }

        public int? RuleMinQuantity { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Loại chiết khấu
        /// percentage: phần trăm, fixed_amount: tiền cố định
        /// </summary>
        public string DiscountType { get; set; }

        public decimal? DiscountPercentage { get; set; }

        public decimal? DiscountFixedAmount { get; set; }

        /// <summary>
        /// Sản phẩm cho dòng chiết khấu
        /// </summary>
        public Guid? DiscountLineProductId { get; set; }
        public Product DiscountLineProduct { get; set; }

        /// <summary>
        /// Thời gian hiệu lực khi coupon đc tạo ra: ngày
        /// </summary>
        public int? ValidityDuration { get; set; }

        /// <summary>
        /// Không áp dụng cộng dồn khuyến mãi
        /// </summary>
        public bool? NotIncremental { get; set; }
        public ICollection<SaleCouponProgramProductRel> DiscountSpecificProducts { get; set; } = new List<SaleCouponProgramProductRel>();
        public ICollection<SaleCouponProgramProductCategoryRel> DiscountSpecificProductCategories { get; set; } = new List<SaleCouponProgramProductCategoryRel>();
        public ICollection<SaleCoupon> Coupons { get; set; } = new List<SaleCoupon>();

        public ICollection<SaleOrderLine> SaleLines { get; set; } = new List<SaleOrderLine>();

        public string RewardType { get; set; }

        /// <summary>
        /// A promotional program can be either a limited promotional offer without code (applied automatically)
        /// or with a code (displayed on a magazine for example) that may generate a discount on the current
        /// order or create a coupon for a next order.
        /// 
        /// A coupon program generates coupons with a code that can be used to generate a discount on the current
        /// order or create a coupon for a next order.
        /// 
        /// coupon_program: Chương trình coupon
        /// promotion_program: Chương trình khuyến mãi
        /// </summary>
        public string ProgramType { get; set; }

        /// <summary>
        /// ('no_code_needed', 'Automatically Applied')
        /// ('code_needed', 'Use a code')
        /// Automatically Applied - No code is required, if the program rules are met, the reward is applied (Except the global discount or the free shipping rewards which are not cumulative)
        /// Use a code - If the program rules are met, a valid code is mandatory for the reward to be applied
        /// </summary>
        public string PromoCodeUsage { get; set; }

        public string PromoCode { get; set; }

        /// <summary>
        /// on_current_order: Áp dụng cho đơn hàng hiện tại
        /// on_next_order: Áp dụng cho đơn hàng tiếp thep
        /// </summary>
        public string PromoApplicability { get; set; }

        /// <summary>
        /// Mức giảm tối đa
        /// </summary>
        public decimal? DiscountMaxAmount { get; set; }

        public Guid? RewardProductId { get; set; }
        public Product RewardProduct { get; set; }

        public int? RewardProductQuantity { get; set; }

        /// <summary>
        /// Ngày bắt đầu
        /// </summary>
        public DateTime? RuleDateFrom { get; set; }

        /// <summary>
        /// Ngày kết thúc
        /// </summary>
        public DateTime? RuleDateTo { get; set; }

        public string RewardDescription { get; set; }

        /// <summary>
        /// on_order : phiếu điều trị
        /// specific_product_categories: nhóm dịch vụ
        /// specific_products: dịch vụ
        /// </summary>
        public string DiscountApplyOn { get; set; }

        /// <summary>
        /// Chọn các thứ trong tuần
        /// </summary>
        public string Days { get; set; }
    }
}
