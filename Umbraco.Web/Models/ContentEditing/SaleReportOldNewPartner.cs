using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportOldNewPartnerInput
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class SaleReportOldNewPartnerDetails
    {
        public string Location { get; set; }
        public int PartnerTotal { get; set; }
        public int PartnerOld { get; set; }
        public int PartnerNew { get; set; }
    }

    public class SaleReportOldNewPartnerOutput
    {
        public int PartnerTotal { get; set; }
        public int PartnerOld { get; set; }
        public int PartnerNew { get; set; }
        public IEnumerable<SaleReportOldNewPartnerDetails> Details { get; set; } = new List<SaleReportOldNewPartnerDetails>();
    }
}
