using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountFinancialReportAccountAccountTypeRel
    {
        public Guid AccountTypeId { get; set; }
        public AccountAccountType AccountType { get; set; }

        public Guid FinancialReportId { get; set; }
        public AccountFinancialReport FinancialReport { get; set; }
    }
}