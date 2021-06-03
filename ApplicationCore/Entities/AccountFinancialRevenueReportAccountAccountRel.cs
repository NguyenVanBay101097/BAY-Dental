using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountFinancialRevenueReportAccountAccountRel
    {
        public Guid AccountId { get; set; }
        public AccountAccount Account { get; set; }

        public Guid FinancialReportId { get; set; }
        public AccountFinancialRevenueReport FinancialRevenueReport { get; set; }
    }
}