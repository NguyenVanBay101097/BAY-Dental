using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Phiếu điều trị / phiếu tư vấn
    /// </summary>
    public class SaleOrder: BaseEntity
    {
        public SaleOrder()
        {
            State = "draft";
            DateOrder = DateTime.Now;
            Name = "/";
            IsQuotation = false;
            InvoiceStatus = "no";
            IsFast = false;
        }

        public SaleOrder(SaleOrder order)
        {
            PartnerId = order.PartnerId;
            AmountUntaxed = order.AmountUntaxed;
            AmountTax = order.AmountTax;
            AmountTotal = order.AmountTotal;
            Note = order.Note;
            CompanyId = order.CompanyId;
            UserId = order.UserId;
            InvoiceStatus = order.InvoiceStatus;
            Residual = order.Residual;
            PricelistId = order.PricelistId;
            State = "draft";
            DateOrder = DateTime.Now;
            Name = "/";
            IsQuotation = false;
            IsFast = false;
        }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// ngày hoàn thành
        /// </summary>
        public DateTime? DateDone { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }


        public decimal? AmountTax { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTotal { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Quotation
        /// sale: Sales Order
        /// done: Locked
        /// cancel: Cancelled
        /// </summary>
        public string State { get; set; }

        public string Name { get; set; }

        public ICollection<SaleOrderLine> OrderLines { get; set; } = new List<SaleOrderLine>();

        public ICollection<DotKham> DotKhams { get; set; } = new List<DotKham>();

        public ICollection<SaleOrderPayment> SaleOrderPayments { get; set; } = new List<SaleOrderPayment>();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string InvoiceStatus { get; set; }

        public decimal? Residual { get; set; }


        /// <summary>
        /// Ghi nhận lại sale order có sử dụng thẻ thành viên
        /// </summary>
        public Guid? CardId { get; set; }
        public CardCard Card { get; set; }

        public Guid? PricelistId { get; set; }
        public ProductPricelist Pricelist { get; set; }

        /// <summary>
        /// Phân loại phiếu tư vấn và phiếu điều trị: quotation, sale_order
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Là phiếu tư vấn
        /// </summary>
        public bool? IsQuotation { get; set; }

        /// <summary>
        /// Cho phiếu điều trị tham chiếu đến phiếu tư vấn
        /// </summary>
        public Guid? QuoteId { get; set; }
        public SaleOrder Quote { get; set; }

        /// <summary>
        /// Cho phiếu tư vấn tham chiếu đến phiếu điều trị
        /// </summary>
        public Guid? OrderId { get; set; }
        public SaleOrder Order { get; set; }

        //public Guid? EmployeeId { get; set; }
        //public Employee Employee { get; set; }

        public Guid? CodePromoProgramId { get; set; }
        public SaleCouponProgram CodePromoProgram { get; set; }

        public ICollection<SaleOrderNoCodePromoProgram> NoCodePromoPrograms { get; set; } = new List<SaleOrderNoCodePromoProgram>();

        public ICollection<SaleCoupon> AppliedCoupons { get; set; } = new List<SaleCoupon>();

        public ICollection<SaleCoupon> GeneratedCoupons { get; set; } = new List<SaleCoupon>();

        public ICollection<SaleOrderServiceCardCardRel> SaleOrderCardRels { get; set; } = new List<SaleOrderServiceCardCardRel>();

        public bool IsFast { get; set; }

        public Guid? JournalId { get; set; }
        public AccountJournal Journal { get; set; }
        /// <summary>
        /// list phân việc khảo sát
        /// </summary>
        public ICollection<SurveyAssignment> Assignments { get; set; } = new List<SurveyAssignment>();

        /// <summary>
        /// danh sách chương trình ưu đãi
        /// </summary>
        public ICollection<SaleOrderPromotion> Promotions { get; set; } = new List<SaleOrderPromotion>();

        public Guid? QuotationId { get; set; }
        public Quotation Quotation { get; set; }
    }
}
