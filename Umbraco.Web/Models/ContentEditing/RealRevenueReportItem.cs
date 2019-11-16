using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RealRevenueReportItem
    {
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

        public int? Year { get; set; }
        public int? WeekOfYear { get; set; }
        public int? QuarterOfYear { get; set; }

        public string GroupBy { get; set; }
        public Guid? PartnerId { get; set; }
    }
}
