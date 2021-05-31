using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class QuotationPromotionLineBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Đơn giá phẩn bổ = Amount / Số lượng
        /// </summary>
        public decimal PriceUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class QuotationPromotionLineSave
    {
        public Guid Id { get; set; }

        public Guid QuotationLineId { get; set; }

        /// <summary>
        /// Đơn giá phẩn bổ = Amount / Số lượng
        /// </summary>
        public double PriceUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class QuotationPromotionLineDisplay
    {
        public Guid Id { get; set; }

        public Guid QuotationLineId { get; set; }
        public QuotationLineDisplay QuotationLine { get; set; }

        /// <summary>
        /// Đơn giá phẩn bổ = Amount / Số lượng
        /// </summary>
        public decimal PriceUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }
}
