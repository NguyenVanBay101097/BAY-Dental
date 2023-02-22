using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountMoveLineReport
    {
        public string Name { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        //public DateTime? Date { get; set; }
        //public Guid PartnerId { get; set; }
        //public PartnerSimple Partner { get; set; }
    }

    public class AccountMoveLineReportPaged
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
