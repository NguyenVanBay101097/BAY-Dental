using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportItem
    {
        public string Name { get; set; }
        public decimal ProductUOMQty { get; set; }
        public decimal PriceTotal { get; set; }
        public DateTime? Date { get; set; }
        public int? Year { get; set; }
        public int? WeekOfYear { get; set; }
        public int? QuarterOfYear { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string State { get; set; }
        public string GroupBy { get; set; }
        public Guid? PartnerId { get; set; }
        public string UserId { get; set; }
        public Guid? ProductId { get; set; }
        public bool? IsQuotation { get; set; }
    }
}
