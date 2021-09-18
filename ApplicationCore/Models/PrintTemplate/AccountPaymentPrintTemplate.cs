using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class AccountPaymentPrintTemplate
    {
        public string CompanyName { get; set; }

        public string CompanyCity { get; set; }

        public string CompanyDistrict { get; set; }

        public string CompanyWard { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyPhone { get; set; }

        public string CompanyEmail { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyLogo { get; set; }

        public string PartnerRef { get; set; }

        public string PartnerName { get; set; }

        public string PartnerDisplayName { get; set; }

        public string PartnerCity { get; set; }

        public string PartnerDistrict { get; set; }

        public string PartnerType { get; set; }

        public string PartnerWard { get; set; }

        public string PartnerStreet { get; set; }

        public string PartnerAddress { get; set; }

        public string PartnerPhone { get; set; }

        public DateTime PaymentDate { get; set; }

        public string JournalName { get; set; }

        public decimal Amount { get; set; }

        public string AmountText { get; set; }

        public string Communication { get; set; }

        public string PaymentType { get; set; }
        public string PaymentName { get; set; }

        public string UserName { get; set; }
    }
}
