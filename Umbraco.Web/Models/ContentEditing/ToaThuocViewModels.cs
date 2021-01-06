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
        public string  PartnerName { get; set; }

        public Guid SaleOrderId { get; set; }
        public string SaleOrderName { get; set; }

        public string EmployeeName { get; set; }

        public string Diagnostic { get; set; }
        public Guid PartnerId { get; set; }

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
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public int? Sequence { get; set; }
    }
}
