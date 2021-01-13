using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleReportPartnerItem
    {
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public string PartnerName { get; set; }

        public string PartnerPhone { get; set; }

        public int? OrderCount { get; set; }

     

        public DateTime? LastDateOrder { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerDisplayName { get; set; }
    }

    public class SaleReportPartnerItemV3
    {
        public int Year { get; set; }
        public int WeekOfYear { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int TotalNewPartner { get; set; }
        public int TotalOldPartner { get; set; }
        public IEnumerable<SaleReportPartnerV3Detail> lines { get; set; } = new List<SaleReportPartnerV3Detail>();
    }

    public class SaleReportPartnerV3Detail
    {
        public Guid PartnerId { get; set; }
        public DateTime Date { get; set; }
        public string PartnerName{ get; set; }
        public string OrderName { get; set; }
        public int CountLine { get; set; }
        public string Type { get; set; }
    }
}
