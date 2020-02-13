using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RealRevenueReportResult
    {
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

        public decimal? TotalAmountResidual { get; set; }

        public IEnumerable<RealRevenueReportItem> Items { get; set; } = new List<RealRevenueReportItem>();
    }

    public class RealRevenueReportItem
    {
        public string Name { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

        public decimal? AmountResidual { get; set; }
    }
}
