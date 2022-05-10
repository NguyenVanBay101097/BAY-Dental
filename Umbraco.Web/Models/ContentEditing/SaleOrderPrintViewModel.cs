using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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

        public Guid PartnerId { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public SaleOrderPartnerPrintVM Partner { get; set; }

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

        public IEnumerable<SaleOrderOrderLinePrintVM> OrderLines { get; set; } = new List<SaleOrderOrderLinePrintVM>();

        public ICollection<DotKham> DotKhams { get; set; } = new List<DotKham>();

        public IEnumerable<SaleOrderOrderPaymentPrintVM> SaleOrderPayments { get; set; } = new List<SaleOrderOrderPaymentPrintVM>();

        public ICollection<SaleOrderPaymentRel> SaleOrderPaymentRels { get; set; } = new List<SaleOrderPaymentRel>();

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string InvoiceStatus { get; set; }

        public decimal? Residual { get; set; }

        /// <summary>
        /// bác sĩ đại diện
        /// </summary>
        public Guid? DoctorId { get; set; }
        public Employee Doctor { get; set; }

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

        public decimal? AmountUndiscountTotal
        {
            get
            {
                return OrderLines.Sum(x => x.PriceUnit * x.ProductUomqty);
            }
        }

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

    public class SaleOrderPartnerPrintVM
    {
        public string Name { get; set; }

        public string Gender { get; set; }

        public string GetGender
        {
            get
            {
                if (Gender == "male")
                    return "Nam";
                else if (Gender == "female")
                    return "Nữ";
                else
                    return "Khác";
            }
            set { }
        }

        public int? BirthYear { get; set; }

        public int? BirthMonth { get; set; }

        public int? BirthDay { get; set; }

        public string GetBirthDay
        {
            get
            {
                if (!BirthDay.HasValue && !BirthMonth.HasValue && !BirthYear.HasValue) return "";
                return $"{(BirthDay.HasValue ? BirthDay.Value.ToString() : "--")}/" +
                    $"{(BirthMonth.HasValue ? BirthMonth.Value.ToString() : "--")}/" +
                    $"{(BirthYear.HasValue ? BirthYear.Value.ToString() : "----")}";
            }
        }

        public string Phone { get; set; }

        public string Address
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrEmpty(Street))
                    list.Add(Street);
                if (!string.IsNullOrEmpty(WardName))
                    list.Add(WardName);
                if (!string.IsNullOrEmpty(DistrictName))
                    list.Add(DistrictName);
                if (!string.IsNullOrEmpty(CityName))
                    list.Add(CityName);
                return string.Join(", ", list);
            }
            set { }
        }

        public string Street { get; set; }

        public string WardName { get; set; }

        public string DistrictName { get; set; }

        public string CityName { get; set; }

        /// <summary>
        /// Danh sách tiểu sử bệnh
        /// </summary>
        public string HistoryList
        {
            get
            {
                return string.Join(", ", Histories);
            }
        }

        public IEnumerable<string> Histories { get; set; } = new List<string>();

        public string MedicalHistory { get; set; }
    }

    public class SaleOrderOrderLinePrintVM
    {
        public SaleOrderOrderLineProductPrintVM Product { get; set; }

        public string Diagnostic { get; set; }

        public string TeethDisplay
        {
            get
            {
                return string.Join(", ", Teeth);
            }
        }

        public IEnumerable<string> Teeth { get; set; } = new List<string>();

        public decimal ProductUomqty { get; set; }

        public decimal PriceUnit { get; set; }
    }

    public class SaleOrderOrderLineProductPrintVM
    {
        public string Name { get; set; }
    }

    public class SaleOrderOrderPaymentPrintVM
    {
        public decimal Amount { get; set; }

        public string LinesDisplay
        {
            get
            {
                return string.Join(", ", LinesName);
            }
        }

        public IEnumerable<string> LinesName { get; set; } = new List<string>();

        public IEnumerable<SaleOrderOrderPaymentRelPrintVM> PaymentRels { get; set; } = new List<SaleOrderOrderPaymentRelPrintVM>();
        
    }

    public class SaleOrderOrderPaymentRelPrintVM
    {
        public SaleOrderOrderPaymentRelPaymentPrintVM Payment { get; set; }
    }

    public class SaleOrderOrderPaymentRelPaymentPrintVM
    {
        public string Name { get; set; }

        public DateTime PaymentDate { get; set; }

        public SaleOrderOrderPaymentRelPaymentJournalPrintVM Journal { get; set; }

        public decimal Amount { get; set; }
    }

    public class SaleOrderOrderPaymentRelPaymentJournalPrintVM
    {
        public string Name { get; set; }
    }
}
