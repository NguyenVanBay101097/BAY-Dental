using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class AccountingReport
    {
        public AccountingReport()
        {
            TargetMove = "posted";
        }

        public Guid? AccountReportId { get; set; }

        public bool? DebitCredit { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public DateTime? DateTo { get; set; }

        public string TargetMove { get; set; }
    }
}
