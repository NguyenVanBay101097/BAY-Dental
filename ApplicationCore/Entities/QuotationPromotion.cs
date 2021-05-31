using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class QuotationPromotion : BaseEntity
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// Xác định ưu đãi cho phiếu báo giá
        /// </summary>
        public Guid? QuotationId { get; set; }
        public Quotation Quotation { get; set; }

        /// <summary>
        /// Ưu đãi trên dịch vụ
        /// </summary>
        public Guid? QuotationLineId { get; set; }
        public QuotationLine QuotationLine { get; set; }

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


        public ICollection<QuotationPromotionLine> Lines { get; set; } = new List<QuotationPromotionLine>();
    }
}
