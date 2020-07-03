using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerPrintProfileVM
    {
        public CompanyPrintVM Company { get; set; }

        public PartnerPrintVM Partner { get; set; }

        public IEnumerable<PartnerPrintProfileService> ServiceList { get; set; } = new List<PartnerPrintProfileService>();

        public IEnumerable<PartnerPrintProfilePayment> PaymentList { get; set; } = new List<PartnerPrintProfilePayment>();

    }

    public class PartnerPrintProfileService
    {
        public DateTime DateOrder { get; set; }

        public string OrderPartner { get; set; }

        public string ProductName { get; set; }
    }

    public class PartnerPrintProfilePayment
    {
        public DateTime PaymentDate { get; set; }

        public string JournalName { get; set; }

        public string Amount { get; set; }
    }
}
