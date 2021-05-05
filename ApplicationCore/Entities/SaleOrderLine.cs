using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Chi tiết bán dịch vụ
    /// </summary>
    public class SaleOrderLine : BaseEntity
    {
        public SaleOrderLine()
        {
            State = "draft";
            InvoiceStatus = "no";
            DiscountType = "percentage";
            DiscountFixed = 0;
            Sequence = 10;
            QtyToInvoice = 0;
            QtyInvoiced = 0;
            AmountPaid = 0;
            AmountResidual = 0;
        }

        public SaleOrderLine(SaleOrderLine line)
        {
            State = "draft";
            InvoiceStatus = "no";
            PriceUnit = line.PriceUnit;
            ProductUOMQty = line.ProductUOMQty;
            Name = line.Name;
            OrderPartnerId = line.OrderPartnerId;
            OrderId = line.OrderId;
            ProductUOMId = line.ProductUOMId;
            Discount = line.Discount;
            ProductId = line.ProductId;
            CompanyId = line.CompanyId;
            PriceSubTotal = line.PriceSubTotal;
            PriceTax = line.PriceTax;
            PriceTotal = line.PriceTotal;
            SalesmanId = line.SalesmanId;
            Note = line.Note;
            ToothCategoryId = line.ToothCategoryId;
            Diagnostic = line.Diagnostic;
            Sequence = line.Sequence;
            DiscountType = "percentage";
            DiscountFixed = 0;
            Sequence = 10;
            QtyToInvoice = 0;
            QtyInvoiced = 0;
            AmountPaid = 0;
            AmountResidual = 0;
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

        /// <summary>
        /// Dùng để xử lý trường hợp khuyến mãi
        /// </summary>
        public decimal? AmountToInvoice { get; set; }

        public decimal? AmountInvoiced { get; set; }

        public ICollection<SaleOrderLineInvoiceRel> SaleOrderLineInvoiceRels { get; set; } = new List<SaleOrderLineInvoiceRel>();

        public Guid? ToothCategoryId { get; set; }
        public ToothCategory ToothCategory { get; set; }

        /// <summary>
        /// whole_jaw: nguyên hàm
        /// upper_jaw : hàm trên
        /// lower_jaw : hàm dưới
        /// manual :  thủ công
        /// </summary>
        public string ToothType { get; set; }

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

        public bool IsRewardLine { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }

        public decimal? PriceReduce { get; set; }

        /// <summary>
        /// Số tiền đã thanh toán
        /// </summary>
        public decimal? AmountPaid { get; set; }

        /// <summary>
        /// Không sử dụng
        /// </summary>
        public decimal? AmountResidual { get; set; }

        /// <summary>
        /// Tổng đơn giá giảm của dịch vụ
        /// </summary>
        public double? AmountDiscountTotal { get; set; }

        /// <summary>
        /// bác sĩ được hưởng hoa hồng
        /// </summary>
        //public Guid? PartnerCommissionId { get; set; }
        //public SaleOrderLinePartnerCommission PartnerCommission { get; set; }

        public ICollection<SaleOrderLinePartnerCommission> PartnerCommissions { get; set; } = new List<SaleOrderLinePartnerCommission>();

        public ICollection<SaleOrderLineInvoice2Rel> SaleOrderLineInvoice2Rels { get; set; } = new List<SaleOrderLineInvoice2Rel>();

        public ICollection<SaleOrderLinePaymentRel> SaleOrderLinePaymentRels { get; set; } = new List<SaleOrderLinePaymentRel>();

        public ICollection<LaboOrder> Labos { get; set; } = new List<LaboOrder>();

        /// <summary>
        /// danh sách chương trình ưu đãi
        /// </summary>
        public ICollection<SaleOrderPromotion> Promotions { get; set; } = new List<SaleOrderPromotion>();

        public ICollection<SaleOrderPromotionLine> PromotionLines { get; set; } = new List<SaleOrderPromotionLine>();

        /// <summary>
        /// Xác định line bị hủy bỏ
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Phụ tá
        /// </summary>
        public Guid? AssistantId { get; set; }
        public Employee Assistant { get; set; }

        /// <summary>
        /// người tư vấn
        /// </summary>
        public Guid? CounselorId { get; set; }
        public Employee Counselor { get; set; }

        public Guid? AdvisoryId { get; set; }
        public Advisory Advisory { get; set; }

        /// <summary>
        /// ngừng hoạt động
        /// </summary>
        public bool IsActive { get; set; }

    }
}
