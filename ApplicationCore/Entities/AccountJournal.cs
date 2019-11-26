using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountJournal: BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public Guid? DefaultDebitAccountId { get; set; }
        public AccountAccount DefaultDebitAccount { get; set; }

        public Guid? DefaultCreditAccountId { get; set; }
        public AccountAccount DefaultCreditAccount { get; set; }

        /// <summary>
        /// Allow Cancelling Entries
        /// </summary>
        public bool UpdatePosted { get; set; }

        public Guid SequenceId { get; set; }
        public IRSequence Sequence { get; set; }

        /// <summary>
        /// Mã phát sinh trường hợp trả hàng
        /// </summary>
        public Guid? RefundSequenceId { get; set; }
        public IRSequence RefundSequence { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Xác định có nên dùng riêng mã phát sinh trả hàng
        /// </summary>
        public bool DedicatedRefund { get; set; }

        public Guid? BankAccountId { get; set; }
        public ResPartnerBank BankAccount { get; set; }
    }
}
