using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountReportGeneralLedger
    {
        public AccountReportGeneralLedger()
        {
            InitialBalance = false;
            SortBy = "sort_date";
            TargetMove = "posted";
            DislayAccount = "movement";
        }
        public bool? InitialBalance { get; set; }

        public string SortBy { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public DateTime? DateTo { get; set; }

        public string TargetMove { get; set; }

        public IEnumerable<Guid> AccountIds { get; set; } = new List<Guid>();

        public string DislayAccount { get; set; }
    }
}
