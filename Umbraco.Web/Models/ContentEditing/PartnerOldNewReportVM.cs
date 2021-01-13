using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerOldNewReportVM
    {
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public int TotalNewPartner { get; set; }
        public int TotalOldPartner { get; set; }
        public IEnumerable<PartnerOldNewReportVMDetail> OrderLines { get; set; } = new List<PartnerOldNewReportVMDetail>();
    }

    public class PartnerOldNewReportVMDetail
    {
        public Guid PartnerId { get; set; }
        public DateTime Date { get; set; }
        public string PartnerName { get; set; }
        public string OrderName { get; set; }
        public int CountLine { get; set; }
        public string Type { get; set; }
    }

    public class PartnerOldNewReportSearch
    {
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public DateTime? DateTo { get; set; }

    }
}
