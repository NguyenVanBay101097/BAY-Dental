using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportPartnerSearch
    {
        public string State { get; set; }

        /// <summary>
        /// old: Khách cũ, new: Khách mới, all: Tất cả
        /// </summary>
        public string PartnerDisplay { get; set; }

        public int? MonthsFrom { get; set; }

        public int? MonthsTo { get; set; }
        public string Search { get; set; }
    }
}
