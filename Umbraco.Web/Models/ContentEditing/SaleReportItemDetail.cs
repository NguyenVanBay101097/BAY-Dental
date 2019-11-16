using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportItemDetail
    {
        public string ProductName { get; set; }

        public decimal ProductUOMQty { get; set; }

        public decimal? PriceTotal { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }
    }
}
