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

        public string Note { get; set; }
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

        public Guid? PartnerId { get; set; }
    }

    public class ToaThuocDisplay: ToaThuocBasic
    {
        public ToaThuocDisplay()
        {
            Date = DateTime.Now;
        }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Liên kết với đợt khám nào?
        /// </summary>
        public Guid? DotKhamId { get; set; }
        public DotKhamSimple DotKham { get; set; }

        /// <summary>
        /// Tài khoản tạo toa thuốc này
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public Guid CompanyId { get; set; }

        public IEnumerable<ToaThuocLineDisplay> Lines { get; set; } = new List<ToaThuocLineDisplay>();
    }

    public class ToaThuocDefaultGet
    {
        public Guid? DotKhamId { get; set; }
    }

    public class ToaThuocLineDefaultGet
    {
        public Guid? ProductId { get; set; }
    }

    public class ToaThuocPrintViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string Name { get; set; }
        public string PartnerName { get; set; }
        public string PartnerGender { get; set; }
        public string PartnerAge { get; set; }
        public string PartnerAddress { get; set; }
        public DateTime Date { get; set; }

        public string Note { get; set; }

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
