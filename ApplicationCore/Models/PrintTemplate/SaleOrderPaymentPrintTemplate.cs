using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class SaleOrderPaymentPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }
        public PartnerPrintTemplate OrderPartner { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePayment { get; set; }
        public string JournalName { get; set; }
        public string Note { get; set; }
        public ApplicationUserPrintTemplate User { get; set; }
    }
}
