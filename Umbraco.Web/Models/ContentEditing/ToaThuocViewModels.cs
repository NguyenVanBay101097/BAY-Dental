using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ToaThuocBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime Date { get; set; }
        public string PartnerName { get; set; }
        public string PartnerDisplayName { get; set; }

        public Guid SaleOrderId { get; set; }
        public string SaleOrderName { get; set; }

        public string EmployeeName { get; set; }

        public string Diagnostic { get; set; }
        public Guid PartnerId { get; set; }
        public string InvoiceStatus { get; set; }

    }

    public class ToaThuocPaged
    {
        public ToaThuocPaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public Guid? PartnerId { get; set; }
        public Guid? SaleOrderId { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class ToaThuocDisplay
    {
        public ToaThuocDisplay()
        {
            Date = DateTime.Now;
        }
        public Guid PartnerId { get; set; }
        public PartnerCustomerDonThuoc Partner { get; set; }
        public Guid? DotKhamId { get; set; }
        public DotKhamSimple DotKham { get; set; }
        public Guid? EmployeeId { get; set; }
        public EmployeeBasic Employee { get; set; }

        public IEnumerable<ToaThuocLineDisplay> Lines { get; set; } = new List<ToaThuocLineDisplay>();
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public Guid CompanyId { get; set; }
        public Guid? SaleOrderId { get; set; }
        public string Name { get; set; }
        public DateTime? ReExaminationDate { get; set; }

        public string Diagnostic { get; set; }
    }

    public class ToaThuocDefaultGet
    {
        public Guid? DotKhamId { get; set; }
        public Guid? PartnerId { get; set; }
        public Guid? SaleOrderId { get; set; }
    }

    public class ToaThuocLineDefaultGet
    {
        public Guid? ProductId { get; set; }
    }

    public class ToaThuocPrintViewModel
    {
        public CompanyPrintVM Company { get; set; }

        public PartnerPrintVM Partner { get; set; }

        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public string Diagnostic { get; set; }
        public string EmployeeName { get; set; }

        public DateTime? ReExaminationDate { get; set; }

        public IEnumerable<ToaThuocLinePrintViewModel> Lines { get; set; } = new List<ToaThuocLinePrintViewModel>();
    }

    public class ToaThuocLinePrintViewModel
    {
        public string ProductName { get; set; }
        public string ProductUOMName { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public int? Sequence { get; set; }
        public int NumberOfTimes { get; set; }
        public int NumberOfDays { get; set; }
        public decimal AmountOfTimes { get; set; }
        public string UseAt { get; set; }
        public string Note { get; set; }
        public string GetUseAtDisplay
        {
            get
            {
                switch (UseAt)
                {
                    case "after_meal": return "Sau khi ăn";
                    case "before_meal": return "Trước khi ăn";
                    case "in_meal": return "Trong khi ăn";
                    case "after_wakeup": return "Sau khi dậy";
                    case "before_sleep": return "Trước khi đi ngủ";
                    case "other": return (string.IsNullOrEmpty(Note) ? "Khác" : Note);
                    default:
                        return "";
                }
            }
            set
            {

            }
        }
    }
}
