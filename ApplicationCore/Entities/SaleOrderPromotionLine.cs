using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderPromotionLine : BaseEntity
    {
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        public Guid PromotionId { get; set; }
        public SaleOrderPromotion Promotion { get; set; }

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
