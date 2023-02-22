using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class JournalReport
    {
        public decimal DebitSum { get; set; }

        public decimal CreditSum { get; set; }
        public decimal BalanceSum { get; set; }

        public Guid? JournalId { get; set; }

        public string Name { get; set; }

        public int? Year { get; set; }
        public int? Week { get; set; }
        public int? Quarter { get; set; }

        public DateTime? Date { get; set; }

        public string GroupBy { get; set; }
    }

    public class JournalReportPaged
    {
        public JournalReportPaged()
        {
            GroupBy = "journal";
        }


        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string Search { get; set; }
        public string GroupBy { get; set; }
        /// <summary>
        /// all : tất cả
        /// bank : tài khoản ngân hàng
        /// cash : tiền mặt
        /// </summary>
        public string Filter { get; set; }

    }

    public class JournalReportDetailPaged
    {
        public Guid? JournalId { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string GroupBy { get; set; }
    }

    public class JournalCashBankReportSearch
    {
        public JournalCashBankReportSearch()
        {
            ResultSelection = "cash_bank";
        }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public string ResultSelection { get; set; }
     
    }

    public class ReportJournalData
    {
        public string TargetMove { get; set; }

        public int MyProperty { get; set; }
    }

    public class ReportJournalItem
    {
        public AccountJournalBasic Journal { get; set; }

        public decimal Begin { get; set; }

        public decimal SumDebit { get; set; }

        public decimal SumCredit { get; set; }

        public decimal End
        {
            get
            {
                return Begin + SumDebit - SumCredit;
            }
            set { }
        }

        public IEnumerable<AccountMoveLineBasic> Lines { get; set; } = new List<AccountMoveLineBasic>();
    }

    public class ReportCashBankJournalSearch
    {
        /// <summary>
        /// cash_bank : tất cả
        /// bank : tài khoản ngân hàng
        /// cash : tiền mặt
        /// </summary>
        public string ResultSelection { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }
}
