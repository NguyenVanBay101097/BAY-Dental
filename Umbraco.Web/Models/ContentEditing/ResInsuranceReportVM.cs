using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ImsuranceDebtFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? InsuranceId { get; set; }
    }

    public class ImsuranceDebtReport
    {
        public string PartnerName { get; set; }
        public DateTime? Date { get; set; }

        public decimal AmountTotal { get; set; }

        public string Origin { get; set; }

        public Guid MoveId { get; set; }
        public string MoveType { get; set; }
    }
}
