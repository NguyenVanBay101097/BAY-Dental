using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// discount: ProductId, ko ProductId
    /// coupon: ProductId, SaleProgram
    /// promotion: ProductId, 
    /// </summary>
    public class SaleOrderPromotion : BaseEntity
    {
        public string Name { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// chương trình khuyến mãi , chương trình coupon
        /// </summary>
        public Guid? SaleCouponProgramId { get; set; }
        public SaleCouponProgram SaleCouponProgram { get; set; }

        public Guid? ParentId { get; set; }
        public SaleOrderPromotion Parent { get;set;}

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }

        /// <summary>
        /// Product discount , promotion , coupon
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// discount: giảm tiền
        /// code_usage_program : chương trình sủ dụng mã
        /// promotion_program : chương trình khuyến mãi
        /// </summary>
        public string Type { get; set; }

        public ICollection<SaleOrderPromotion> SaleOrderPromotionChilds { get; set; } = new List<SaleOrderPromotion>();

    }
}
