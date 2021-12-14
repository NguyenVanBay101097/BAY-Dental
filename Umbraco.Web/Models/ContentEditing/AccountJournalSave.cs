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

    public class AccountJournalCreateBankJournalVM
    {
        public Guid BankId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string BankBranch { get; set; }
        public bool Active { get; set; }
    }

    public class AccountJournalUpdateBankJournalVM
    {
        public Guid Id { get; set; }
        public Guid BankId { get; set; }
        public string AccountNumber { get; set; }
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

    public class AccountJournalGetBankJournalVM
    {
        public Guid Id { get; set; }
        public ResBankSimple Bank { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string BankBranch { get; set; }
        public bool Active { get; set; }
    }

    public class AccountJournalBankJournalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BankBic { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string BankBranch { get; set; }
        public bool Active { get; set; }
    }
}
