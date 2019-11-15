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
        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public int QuarterOfYear { get; set; }
    }
}
