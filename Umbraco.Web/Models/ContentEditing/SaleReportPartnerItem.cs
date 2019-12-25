using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportPartnerItem
    {
        public string PartnerName { get; set; }

        public string PartnerPhone { get; set; }

        public int? OrderCount { get; set; }

        public DateTime? LastDateOrder { get; set; }
    }
}
