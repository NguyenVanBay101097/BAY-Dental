using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class VFundBook
    {
        /// <summary>
        /// Bao gom 
        /// SalaryPaymentId, AccountPaymentId, PhieuThuChiId
        /// </summary>
        public Guid ResId { get; set; }

        /// <summary>
        /// SalaryPayment : salary.payment
        /// AccountPayment: account.payment
        /// PhieuThuChi: phieu.thu.chi
        /// </summary>
        public string ResModel { get; set; }

        /// <summary>
        /// ngay tao
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// So phieu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Loai chung tu
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Loai thu chi
        /// </summary>
        public string Type2 { get; set; }

        /// <summary>
        /// So tien
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Trang thai
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Nguoi nhan/ nguoi nop
        /// </summary>
        public string RecipientPayer { get; set; }

        /// <summary>
        /// Tien mat
        /// ngan hang
        /// </summary>
        public string JournalName { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }
        public Company Company { get; set; }
        public Guid CompanyId { get; set; }
    }
}
