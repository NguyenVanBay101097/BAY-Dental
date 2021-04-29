using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPromotionLineBasic
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

    public class SaleOrderPromotionLineSave
    {
        public Guid Id { get; set; }

        public Guid SaleOrderLineId { get; set; }

        /// <summary>
        /// Đơn giá phẩn bổ = Amount / Số lượng
        /// </summary>
        public decimal PriceUnit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class SaleOrderPromotionLineDisplay
    {
        public Guid Id { get; set; }

        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLineDisplay SaleOrderLine { get; set; }

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
