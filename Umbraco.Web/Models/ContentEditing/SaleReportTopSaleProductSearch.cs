using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportTopSaleProductSearch
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        /// <summary>
        /// quantity: So luong
        /// amount: Tong tien
        /// </summary>
        public string TopBy { get; set; }
    }
}
