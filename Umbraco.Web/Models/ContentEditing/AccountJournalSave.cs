using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountJournalSave
    {
        public string Name { get; set; }
        public Guid? BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Type { get; set; }
        public string AccountHolderName { get; set; }
        public string BankBranch { get; set; }
        public bool Active { get; set; }

    }

    public class AccountJournalDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? BankId { get; set; }
        public string AccountNumber { get; set; }
        public string Type { get; set; }
        public string AccountHolderName { get; set; }
        public string BankBranch { get; set; }
        public bool Active { get; set; }

    }

}
