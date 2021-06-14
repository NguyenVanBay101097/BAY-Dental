using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RevenueReportResult
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<RevenueReportResultDetail> Details { get; set; } = new List<RevenueReportResultDetail>();
    }

    public class RevenueReportResultDetail
    {
        public Guid PartnerId { get; set; }
        public string Name { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public int QuarterOfYear { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int? WeekOfYear { get; set; }
    }
}
