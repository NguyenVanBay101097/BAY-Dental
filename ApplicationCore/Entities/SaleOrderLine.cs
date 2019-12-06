using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Chi tiết bán dịch vụ
    /// </summary>
    public class SaleOrderLine: BaseEntity
    {
        public SaleOrderLine()
        {
            State = "draft";
            InvoiceStatus = "no";
        }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public Guid? OrderPartnerId { get; set; }
        public Partner OrderPartner { get; set; }

        public Guid OrderId { get; set; }
        public SaleOrder Order { get; set; }

        public Guid? ProductUOMId { get; set; }
        public UoM ProductUOM { get; set; }

        /// <summary>
        /// %
        /// </summary>
        public decimal Discount { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTax { get; set; }

        public decimal PriceTotal { get; set; }

        public string SalesmanId { get; set; }
        public ApplicationUser Salesman { get; set; }

        public string Note { get; set; }

        public string InvoiceStatus { get; set; }

        public decimal? QtyToInvoice { get; set; }

        public decimal? QtyInvoiced { get; set; }

        public ICollection<SaleOrderLineInvoiceRel> SaleOrderLineInvoiceRels { get; set; } = new List<SaleOrderLineInvoiceRel>();

        public Guid? ToothCategoryId { get; set; }
        public ToothCategory ToothCategory { get; set; }

        /// <summary>
        /// Chẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public ICollection<SaleOrderLineToothRel> SaleOrderLineToothRels { get; set; } = new List<SaleOrderLineToothRel>();

        public ICollection<DotKhamStep> DotKhamSteps { get; set; } = new List<DotKhamStep>();

        public int? Sequence { get; set; }

        /// <summary>
        /// Dùng cho chương trình coupon
        /// </summary>
        public Guid? PromotionProgramId { get; set; }
        public SaleCouponProgram PromotionProgram { get; set; }

        /// <summary>
        /// Dùng cho trường hợp chương trình khuyến mãi
        /// </summary>
        public Guid? PromotionId { get; set; }
        public PromotionProgram Promotion { get; set; }

        public Guid? CouponId { get; set; }
        public SaleCoupon Coupon { get; set; }
    }
}
