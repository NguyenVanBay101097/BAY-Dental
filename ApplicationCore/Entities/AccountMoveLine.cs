using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountMoveLine: BaseEntity
    {
        public string Name { get; set; }

        public decimal? Quantity { get; set; }
        public Guid? ProductUoMId { get; set; }
        public UoM ProductUoM { get; set; }
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string Ref { get; set; }
        public Guid? JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// Ngày đáo hạn
        /// </summary>
        public DateTime DateMaturity { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public DateTime? Date { get; set; }

        public Guid MoveId { get; set; }
        public AccountMove Move { get; set; }

        public Guid AccountId { get; set; }
        public AccountAccount Account { get; set; }

        public decimal AmountResidual { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<AccountPartialReconcile> MatchedDebits { get; set; } = new List<AccountPartialReconcile>();

        public ICollection<AccountPartialReconcile> MatchedCredits { get; set; } = new List<AccountPartialReconcile>();

        public bool Reconciled { get; set; }

        public Guid? PaymentId { get; set; }
        public AccountPayment Payment { get; set; }

        public Guid? FullReconcileId { get; set; }
        public AccountFullReconcile FullReconcile { get; set; }

        public Guid? InvoiceId { get; set; }
        public AccountInvoice Invoice { get; set; }
    }
}
