using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountFinancialReport : BaseEntity
    {
        public AccountFinancialReport()
        {
            DisplayDetail = "detail_flat";
            Type = "sum";
        }

        //ten bao cao
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public AccountFinancialReport Parent { get; set; }

        public ICollection<AccountFinancialReport> Childs { get; set; } = new List<AccountFinancialReport>();

        public int? Level { get; set; }

        public int? Sequence { get; set; }

        // ('sum', 'View'),
        //('account_type', 'Account Type'),
        public string Type { get; set; }

        public ICollection<AccountFinancialReportAccountAccountTypeRel> FinancialReportAccountTypeRels { get; set; } = new List<AccountFinancialReportAccountAccountTypeRel>();

        //For accounts that are typically more debited than credited and that you would like to print as negative amounts in your reports, you should reverse the sign of the balance; e.g.: Expense account. The same applies for accounts that are typically more credited than debited and that you would like to print as positive amounts in your reports; e.g.: Income account.
        public int Sign { get; set; }

        public string DisplayDetail { get; set; }
    }
}