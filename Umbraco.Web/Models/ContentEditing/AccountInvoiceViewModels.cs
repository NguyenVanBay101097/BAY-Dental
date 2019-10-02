using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoicePrint
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }

        public string PartnerRef { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerPhone { get; set; }

        public string DateInvoice { get; set; }
        public string Number { get; set; }

        public IEnumerable<AccountInvoiceLinePrint> InvoiceLines { get; set; } = new List<AccountInvoiceLinePrint>();

        public decimal AmountTotal { get; set; }
    }
}
