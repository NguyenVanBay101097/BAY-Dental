using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class AccountPaymentPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }

        public PartnerPrintTemplate Partner { get; set; }

        public DateTime PaymentDate { get; set; }

        public string JournalName { get; set; }

        public decimal Amount { get; set; }

        public string AmountText
        {
            get
            {

                return AmountToText.amount_to_text(Amount);
            }
            set { }
        }

        public string Communication { get; set; }

        public string PaymentType { get; set; }
        public string Name { get; set; }

        public string UserName { get; set; }
    }
}
