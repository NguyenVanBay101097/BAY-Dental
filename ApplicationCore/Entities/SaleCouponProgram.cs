using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleCouponProgram: BaseEntity
    {
        public SaleCouponProgram()
        {
            Active = true;
            RewardType = "discount";
            PromoApplicability = "on_current_order";
            ProgramType = "coupon_program";
        }

        /// <summary>
        /// Tên chương trình
        /// </summary>
        public string Name { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Số tiền mua tối thiểu
        /// </summary>
        public decimal? RuleMinimumAmount { get; set; }

        public decimal? RuleMinQuantity { get; set; }

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

        public ICollection<SaleCoupon> Coupons { get; set; } = new List<SaleCoupon>();

        public ICollection<SaleOrderLine> SaleLines { get; set; } = new List<SaleOrderLine>();

        public string RewardType { get; set; }

        /// <summary>
        /// coupon_program: Chương trình coupon
        /// promotion_program: Chương trình khuyến mãi
        /// </summary>
        public string ProgramType { get; set; }

        /// <summary>
        /// on_current_order: Áp dụng cho đơn hàng hiện tại
        /// on_next_order: Áp dụng cho đơn hàng tiếp thep
        /// </summary>
        public string PromoApplicability { get; set; }

        public decimal? DiscountMaxAmount { get; set; }
    }
}
