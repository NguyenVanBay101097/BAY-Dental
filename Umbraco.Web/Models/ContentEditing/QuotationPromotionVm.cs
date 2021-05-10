using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class QuotationPromotionPaged
    {
        public QuotationPromotionPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public Guid? QuotationId { get; set; }

        public Guid? QuotationLineId { get; set; }

    }

    public class QuotationPromotionBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid? SaleCouponProgramId { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// discount: giảm tiền
        /// code_usage_program : chương trình sủ dụng mã
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }


    }

    public class QuotationPromotionDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public decimal Amount { get; set; }


        /// <summary>
        /// chương trình khuyến mãi , chương trình coupon
        /// </summary>
        public Guid? SaleCouponProgramId { get; set; }
        public SaleCouponProgramDisplay SaleCouponProgram { get; set; }

        public Guid? QuotationId { get; set; }
        public QuotationDisplay Quotation { get; set; }

        public Guid? QuotationLineId { get; set; }
        public QuotationLineDisplay QuotationLine { get; set; }

        /// <summary>
        /// discount: giảm tiền
        /// code_usage_program : chương trình sủ dụng mã
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }

    }

    public class QuotationPromotionSave
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

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

        public Guid? QuotationId { get; set; }

        public Guid? QuotationLineId { get; set; }

        public IEnumerable<QuotationPromotionLineSave> Lines { get; set; } = new List<QuotationPromotionLineSave>();

        /// <summary>
        /// discount: giảm tiền
        /// coupon_program : chương trình coupon
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }

    }
}
