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
            FilterCmp = "filter_no";
            KeyDisplayDetail = "detail_flat";

        }
        public bool? EnableFilter { get; set; }

        public Guid? AccountReportId { get; set; }
        //public AccountFinancialReport AccountReport { get; set; }

        public string LabelFilter { get; set; }

        public string FilterCmp { get; set; }

        public DateTime? DateFromCmp { get; set; }

        public DateTime? DateToCmp { get; set; }

        public bool? DebitCredit { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }
        //public Company Company { get; set; }

        public DateTime? DateTo { get; set; }

        public string TargetMove { get; set; }

        public string KeyDisplayDetail { get; set; }
        //public ICollection<AccountJournal> Journals { get; set; }
    }
}
