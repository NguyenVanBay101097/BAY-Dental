using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPrintVM
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public CompanyPrintVM Company { get; set; }

        public ApplicationUserSimple User { get; set; }

        public PartnerSimpleInfo Partner { get; set; }

        public string Name { get; set; }
        public DateTime DateOrder { get; set; }

        public IEnumerable<SaleOrderLinePrintVM> OrderLines { get; set; } = new List<SaleOrderLinePrintVM>();

        public IEnumerable<PaymentInfoContentPrintVm> HistoryPayments { get; set; } = new List<PaymentInfoContentPrintVm>();

        public IEnumerable<DotKhamDisplayVm> DotKhams { get; set; } = new List<DotKhamDisplayVm>();

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }
    }

    public class PaymentInfoContentPrintVm
    {
        public string Name { get; set; }

        public string JournalName { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// nội dung
        /// </summary>
        public string Communication { get; set; }

        public DateTime? Date { get; set; }

        public Guid PaymentId { get; set; }

        public Guid MoveId { get; set; }

        public string Ref { get; set; }

        public Guid? AccountPaymentId { get; set; }

        public string PaymentPartnerType { get; set; }
    }


}
