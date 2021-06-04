using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountFinancialRevenueReportAccountAccountRel : BaseEntity
    {
        public string AccountCode { get; set; }

        public Guid FinancialRevenueReportId { get; set; }
        public AccountFinancialRevenueReport FinancialRevenueReport { get; set; }
        /// <summary>
        /// cột sẽ được response from acount move line
        ///1: credit, 2: debit, 3: balance
        /// </summary>
        public int Column { get; set; }
    }
}