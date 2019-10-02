using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountMove: BaseEntity
    {
        public AccountMove()
        {
            Name = "/";
            State = "draft";
            Date = DateTime.Today;
        }

        /// <summary>
        /// Number 
        /// </summary>
        public string Name { get; set; }
        public string Ref { get; set; }
        public DateTime Date { get; set; }
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }
        public string State { get; set; }
        public ICollection<AccountMoveLine> Lines { get; set; } = new List<AccountMoveLine>();

        /// <summary>
        /// Internal Note
        /// </summary>
        public string Narration { get; set; }
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
        public Guid? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public decimal Amount { get; set; }
    }
}
