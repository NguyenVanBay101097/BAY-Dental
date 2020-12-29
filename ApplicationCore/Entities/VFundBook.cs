using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class VFundBook
    {
        public Guid ResId { get; set; }
        public string ResModel { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Type2 { get; set; }
        public decimal Amount { get; set; }
        public string State { get; set; }
        public string RecipientPayer { get; set; }
        public string JournalName { get; set; }
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }
        public Company Company { get; set; }
        public Guid CompanyId { get; set; }
    }
}
