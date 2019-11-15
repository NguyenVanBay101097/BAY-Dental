using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class RealRevenueReportSearch
    {
        public RealRevenueReportSearch()
        {
            GroupBy = "day";
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        /// <summary>
        /// Gom nhóm theo ngày, tuần, tháng, quý
        /// </summary>
        public string GroupBy { get; set; }
        public Guid? CompanyId { get; set; }
    }
}
