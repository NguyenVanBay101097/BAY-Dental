using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CashBookReport
    {
        public CashBookReport()
        {
            Begin = 0;
            TotalThu = 0;
            TotalChi = 0;
            TotalAmount = 0;
        }
        public decimal Begin { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalChi { get; set; }
        public decimal TotalThu { get; set; }
    }

    public class CashBookReportDetail
    {
        public DateTime? Date { get; set; }

        public string PartnerName { get; set; }

        public decimal Amount { get; set; }

        public string AccountName { get; set; }

        public string JournalName { get; set; }

        public string Name { get; set; }

        public string InvoiceOrigin { get; set; }
    }

    public class CashBookReportItem
    {
        public DateTime? Date { get; set; }
        public decimal Begin { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalChi { get; set; }
        public decimal TotalThu { get; set; }
    }
}
