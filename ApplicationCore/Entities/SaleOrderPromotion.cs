using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// discount: ProductId, ko ProductId
    /// coupon: ProductId, SaleProgram
    /// promotion: ProductId, 
    /// Ưu đãi
    /// </summary>
    public class SaleOrderPromotion : BaseEntity
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// Xác định ưu đãi cho phiếu điều trị
        /// </summary>
        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        /// <summary>
        /// Ưu đãi trên dịch vụ
        /// </summary>
        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// percentage : giảm phần trăm
        /// fixed : giảm tiền
        /// </summary>
        public string DiscountType { get; set; }

        public decimal? DiscountPercent { get; set; }

        public decimal? DiscountFixed { get; set; }

        /// <summary>
        /// chương trình khuyến mãi , chương trình coupon
        /// </summary>
        public Guid? SaleCouponProgramId { get; set; }
        public SaleCouponProgram SaleCouponProgram { get; set; }

        /// <summary>
        /// discount: giảm tiền
        /// code_usage_program : chương trình sủ dụng mã
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }
        

        public ICollection<SaleOrderPromotionLine> Lines { get; set; } = new List<SaleOrderPromotionLine>();

    }
}
