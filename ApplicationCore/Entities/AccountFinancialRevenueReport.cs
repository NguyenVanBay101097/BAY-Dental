using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// báo cáo nguồn thu
    /// </summary>
    public class AccountFinancialRevenueReport : BaseEntity
    {
        public AccountFinancialRevenueReport()
        {
            DisplayDetail = "detail_flat";
            Type = "sum";
        }

        /// <summary>
        /// tên danh mục báo cáo
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// thuộc danh mục cha nào
        /// </summary>
        public Guid? ParentId { get; set; }
        public AccountFinancialRevenueReport Parent { get; set; }
        /// <summary>
        /// có những list danh mục con nào
        /// </summary>
        public ICollection<AccountFinancialRevenueReport> Childs { get; set; } = new List<AccountFinancialRevenueReport>();
        /// <summary>
        /// level của danh mục hiện tại
        /// </summary>
        public int? Level { get; set; }
        public int? Sequence { get; set; }

        // ('sum', 'View'),
        //('account_type', 'Account Type'),
        //('account_account', 'Account Account'),
        public string Type { get; set; }

        public ICollection<AccountFinancialRevenueReportAccountAccountTypeRel> FinancialRevenueReportAccountTypeRels { get; set; } = new List<AccountFinancialRevenueReportAccountAccountTypeRel>();
        public ICollection<AccountFinancialRevenueReportAccountAccountRel> FinancialRevenueReportAccountRels { get; set; } = new List<AccountFinancialRevenueReportAccountAccountRel>();

        //For accounts that are typically more debited than credited and that you would like to print as negative amounts in your reports, you should reverse the sign of the balance; e.g.: Expense account. The same applies for accounts that are typically more credited than debited and that you would like to print as positive amounts in your reports; e.g.: Income account.
        public int Sign { get; set; }

        public string DisplayDetail { get; set; }
    }
}