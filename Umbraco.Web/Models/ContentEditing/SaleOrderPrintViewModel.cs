using ApplicationCore.Entities;
using MyERP.Utilities;
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

        public IEnumerable<SaleOrderOrderPaymentPrintVMDotKhamPrintVM> DotKhams { get; set; } = new List<SaleOrderOrderPaymentPrintVMDotKhamPrintVM>();

        public IEnumerable<SaleOrderOrderPaymentPrintVM> SaleOrderPayments { get; set; } = new List<SaleOrderOrderPaymentPrintVM>();

        public Guid CompanyId { get; set; }
        public CompanyPrintVM Company { get; set; }

        public string UserId { get; set; }
        public SaleOrderOrderUserPrintVM User { get; set; }

        public string InvoiceStatus { get; set; }

        public decimal? Residual { get; set; }
        /// <summary>
        /// Tổng thanh toán
        /// </summary>
        public decimal? TotalPaid { get; set; }

        [NotMapped]
        public IEnumerable<SaleOrderOrderIrAttachmentPrintVM> IrAttachments { get; set; } = new List<SaleOrderOrderIrAttachmentPrintVM>();
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

        [NotMapped]
        public decimal? AmountUndiscountTotal
        {
            get
            {
                return OrderLines.Sum(x => x.PriceUnit * x.ProductUOMQty);
            }
        }



        /// <summary>
        /// Tổng tiền giảm giá
        /// </summary>
        [NotMapped]
        public decimal? AmountDiscountTotal
        {
            get
            {
                var total = 0.0M;
                foreach (var item in OrderLines)
                {
                    total += (decimal)(item.AmountDiscountTotal ?? 0) * item.ProductUOMQty;
                }
                return total;
            }
        }
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
        public DateTime DateOrder { get; set; }
        public string Name { get; set; }

        public SaleOrderOrderLineProductPrintVM Product { get; set; }

        public string Diagnostic { get; set; }

        public string TeethDisplay
        {
            get
            {
                return string.Join(", ", Teeth);
            }
        }
        public SaleOrderOrderLineUomPrintVM ProductUom { get; set; }

        public IEnumerable<string> Teeth { get; set; } = new List<string>();

        public decimal ProductUOMQty { get; set; }

        public decimal PriceUnit { get; set; }
        public double? AmountDiscountTotal { get; set; }
        public string ToothType { get; set; }


        [NotMapped]
        public decimal PriceWithDiscount
        {
            get
            {
                return PriceUnit - (decimal)FloatUtils.FloatRound((AmountDiscountTotal ?? 0), precisionDigits: 0);
            }
        }

        /// <summary>
        /// Tổng tiền giảm
        /// </summary>
        [NotMapped]
        public decimal TotalDiscountAmount
        {
            get
            {
                return (decimal)FloatUtils.FloatRound((AmountDiscountTotal ?? 0) * (double)ProductUOMQty, precisionDigits: 0);
            }
        }

        public decimal PriceSubTotal { get; set; }
        [NotMapped]
        public decimal TotalUndiscountAmount
        {
            get
            {
                return ProductUOMQty * PriceUnit;
            }
        }

        public Guid? ToothCategoryId { get; set; }
        public ToothCategoryPrintVM ToothCategory { get; set; }
        public IEnumerable<SaleOrderLineToothRelPrintVM> SaleOrderLineToothRels { get; set; } = new List<SaleOrderLineToothRelPrintVM>();
    }

    public class SaleOrderOrderLineProductPrintVM
    {
        public string Name { get; set; }
    }

    public class SaleOrderOrderLineUomPrintVM
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

    public class SaleOrderOrderPaymentPrintVMDotKhamPrintVM
    {
        public IEnumerable<SaleOrderOrderPaymentPrintVMDotKhamLinePrintVM> Lines { get; set; } = new List<SaleOrderOrderPaymentPrintVMDotKhamLinePrintVM>();
        public DateTime Date { get; set; }
    }

    public class SaleOrderOrderPaymentPrintVMDotKhamLinePrintVM
    {
        public SaleOrderOrderLineProductPrintVM Product { get; set; }
        public string NameStep { get; set; }
        public ICollection<DotKhamLineToothRel> ToothRels { get; set; } = new List<DotKhamLineToothRel>();
        [NotMapped]
        public string TeethDisplay
        {
            get
            {
                return ToothRels.Count > 0 ? string.Join(" ,", ToothRels.Select(x => x.Tooth.Name)) : "";
            }
        }

        public string Note { get; set; }

    }

    public class SaleOrderOrderIrAttachmentPrintVM
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class SaleOrderOrderUserPrintVM
    {
        public string Name { get; set; }
    }

    public class ToothCategoryPrintVM
    {
        public string Name { get; set; }
        public int? Sequence { get; set; }
    }

    public class SaleOrderLineToothRelPrintVM
    {
        public Guid SaleLineId { get; set; }
        public Guid ToothId { get; set; }
    }
}
