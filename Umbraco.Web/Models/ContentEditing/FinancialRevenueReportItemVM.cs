using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class FinancialRevenueReportItem
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public ICollection<FinancialRevenueReportItem> Childs { get; set; } = new List<FinancialRevenueReportItem>();
        public int? Level { get; set; }
        public int? Sequence { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
    }

    public class RevenueReportPar
    {
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        public DateTime? DateTo { get; set; }
    }

    public class ComputeAccountBalanceRes
    {
        public Guid AccountId { get; set; }

        public decimal? Balance { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }
    }

    public class ComputeReportBalanceDictValue
    {
        public ComputeReportBalanceDictValue()
        {
            Account = new Dictionary<Guid, ComputeAccountBalanceRes>();
            Debit = 0;
            Credit = 0;
            Balance = 0;
        }
        public IDictionary<Guid, ComputeAccountBalanceRes> Account { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }

        public decimal? Balance { get; set; }
    }
}
