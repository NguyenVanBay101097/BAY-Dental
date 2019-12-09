using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountJournalBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid? BankAccountId { get; set; }
        public ResPartnerBankBasic BankAccount { get; set; }
    }
}
