using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderLineDisplay
    {
        public SaleOrderLineDisplay()
        {
            State = "draft";
        }

        public Guid Id { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// %
        /// </summary>
        public decimal Discount { get; set; }

        public Guid? ProductId { get; set; }
        public ProductSimple Product { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Chuẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryBasic ToothCategory { get; set; }

        public IEnumerable<ToothDisplay> Teeth { get; set; } = new List<ToothDisplay>();

        public string State { get; set; }

        public int? Sequence { get; set; }

        public Guid? PromotionProgramId { get; set; }

        public Guid? PromotionId { get; set; }

        public Guid? CouponId { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }

        public decimal? QtyInvoiced { get; set; }

        public bool IsRewardLine { get; set; }
    }
}
