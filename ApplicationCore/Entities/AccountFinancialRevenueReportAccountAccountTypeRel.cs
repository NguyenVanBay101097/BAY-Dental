using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountFinancialRevenueReportAccountAccountTypeRel
    {
        public Guid AccountTypeId { get; set; }
        public AccountAccountType AccountType { get; set; }

        public Guid FinancialReportId { get; set; }
        public AccountFinancialRevenueReport FinancialRevenueReport { get; set; }
        /// <summary>
        /// cột sẽ được response from acount move line
        ///1: credit, 2: debit, 3: balance
        /// </summary>
        public int Column { get; set; }
    }
}