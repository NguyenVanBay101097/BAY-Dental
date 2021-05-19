using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class QuotationPromotionLine : BaseEntity
    {
        public Guid? QuotationLineId { get; set; }
        public QuotationLine QuotationLine { get; set; }

        public Guid PromotionId { get; set; }
        public QuotationPromotion Promotion { get; set; }

        /// <summary>
        /// Đơn giá phẩn bổ = Amount / Số lượng
        /// </summary>
        public double PriceUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }
}
