using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RealRevenueReportSearch
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
