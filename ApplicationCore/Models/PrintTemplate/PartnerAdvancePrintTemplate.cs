using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class PartnerAdvancePrintTemplate
    {
        public string Name { get; set; }

        /// <summary>
        /// Ngày tạm ứng
        /// </summary>
        public DateTime Date { get; set; }


        public string CreatedById { get; set; }

        public string UserName { get; set; }

        public string PartnerName { get; set; }

        public string JournalName { get; set; }

        public Guid CompanyId { get; set; }
        public CompanyPrintTemplate Company { get; set; }

        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tạm ứng
        /// </summary>
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string AmountText { get { return AmountToText.amount_to_text(Amount); } set { } }

        public string State { get; set; }

        public string Note { get; set; }
    }
}
