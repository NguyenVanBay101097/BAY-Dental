using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPromotionPaged
    {
        public SaleOrderPromotionPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public Guid? SaleOrderId { get; set; }

        public Guid? SaleOrderLineId { get; set; }

    }

    public class SaleOrderPromotionBasic
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

    public class SaleOrderPromotionDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public decimal Amount { get; set; }


        /// <summary>
        /// chương trình khuyến mãi , chương trình coupon
        /// </summary>
        public Guid? SaleCouponProgramId { get; set; }
        public SaleCouponProgramDisplay SaleCouponProgram { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrderDisplay SaleOrder { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLineDisplay SaleOrderLine { get; set; }

        /// <summary>
        /// discount: giảm tiền
        /// code_usage_program : chương trình sủ dụng mã
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }

    }

    public class SaleOrderPromotionSave
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

        public Guid? SaleOrderId { get; set; }

        public Guid? SaleOrderLineId { get; set; }

        public IEnumerable<SaleOrderPromotionLineSave> Lines { get; set; } = new List<SaleOrderPromotionLineSave>();

        /// <summary>
        /// discount: giảm tiền
        /// coupon_program : chương trình coupon
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }

    }

    public class HistoryPromotionRequest
    {
        public HistoryPromotionRequest()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string SearchOrder { get; set; }
        public string SearchOrderLine { get; set; }

        public Guid? SaleCouponProgramId { get; set; }
    }

    public class HistoryPromotionReponse
    {
        public decimal Amount { get; set; }
        public decimal AmountPromotion { get; set; }
        public DateTime? DatePromotion { get; set; }
        public string PartnerName { get; set; }
        public string SaleOrderName { get; set; }
        public string SaleOrderLineName { get; set; }
    }

}
