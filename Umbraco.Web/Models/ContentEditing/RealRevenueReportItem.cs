using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RealRevenueReportResult
    {
        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal End { get; set; }
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
