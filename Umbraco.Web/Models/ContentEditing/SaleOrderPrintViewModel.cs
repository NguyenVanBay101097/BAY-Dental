using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPrintViewModel
    {
        public Guid Id { get; set; }
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

        public ICollection<SaleOrderLineDisplay> OrderLines { get; set; } = new List<SaleOrderLineDisplay>();

        public ICollection<DotKham> DotKhams { get; set; } = new List<DotKham>();

        public ICollection<SaleOrderPayment> SaleOrderPayments { get; set; } = new List<SaleOrderPayment>();

        public ICollection<SaleOrderPaymentRel> SaleOrderPaymentRels { get; set; } = new List<SaleOrderPaymentRel>();

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
        /// <summary>
        /// bác sĩ đại diện
        /// </summary>
        public Guid? DoctorId { get; set; }
        public Employee Doctor { get; set; }

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

        /// <summary>
        /// Tổng thanh toán
        /// </summary>
        public decimal? TotalPaid { get; set; }

        [NotMapped]
        public ICollection<IrAttachment> IrAttachments { get; set; } = new List<IrAttachment>();
        public IEnumerable<History> Histories { get; set; } = new List<History>();
        public string MedicalHistory { get; set; }

        // răng được chọn
        public IEnumerable<ToothSimple> Teeth { get; set; } = new List<ToothSimple>();
        public IEnumerable<ToothPrint> AdultUpLeftTeeth { get; set; }
        public IEnumerable<ToothPrint> AdultUpRightTeeth { get; set; }
        public IEnumerable<ToothPrint> AdultDownLeftTeeth { get; set; }
        public IEnumerable<ToothPrint> AdultDownRightTeeth { get; set; }
        public IEnumerable<ToothPrint> ChildUpLeftTeeth { get; set; }
        public IEnumerable<ToothPrint> ChildUpRightTeeth { get; set; }
        public IEnumerable<ToothPrint> ChildDownLeftTeeth { get; set; }
        public IEnumerable<ToothPrint> ChildDownRightTeeth { get; set; }
        public string ToothCategory { get; set; }
        public bool HaveChildTeeth { get; set; }
        public bool HaveAdultTeeth { get; set; }

        /// <summary>
        /// Tổng tiền chưa giảm
        /// </summary>
        //[NotMapped]
        //public decimal? AmountUndiscountTotal
        //{
        //    get
        //    {
        //        var total = 0.0M;
        //        foreach (var item in OrderLines)
        //        {
        //            total += item.PriceUnit * item.ProductUOMQty;
        //        }
        //        return total;
        //    }
        //}

        /// <summary>
        /// Tổng tiền giảm giá
        /// </summary>
        //[NotMapped]
        //public decimal? AmountDiscountTotal
        //{
        //    get
        //    {
        //        var total = 0.0M;
        //        foreach (var item in OrderLines)
        //        {
        //            total += (decimal)(item.AmountDiscountTotal ?? 0) * item.ProductUOMQty;
        //        }
        //        return total;
        //    }
        //}
    }
}
