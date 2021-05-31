using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountPayment : BaseEntity
    {
        public AccountPayment()
        {
            State = "draft";
            PaymentDifferenceHandling = "open";
            PaymentDate = DateTime.Today;
        }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Đối tác: Khách hàng hoặc nhà cung cấp...
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// customer
        /// supplier
        /// employee
        /// </summary>
        public string PartnerType { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

        /// <summary>
        /// draft: Nháp
        /// posted: Đã vào sổ
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// inbound: Thu tiền
        /// outbound: Chi tiền
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Số hóa đơn, chứng từ
        /// </summary>
        public string Communication { get; set; }

        public ICollection<AccountInvoicePaymentRel> AccountInvoicePaymentRels { get; set; } = new List<AccountInvoicePaymentRel>();

        public ICollection<SaleOrderPaymentAccountPaymentRel> SaleOrderPaymentAccountPaymentRels { get; set; } = new List<SaleOrderPaymentAccountPaymentRel>();

        public ICollection<AccountMoveLine> MoveLines { get; set; }

        /// <summary>
        /// ('open', 'Keep open'), ('reconcile', 'Mark invoice as fully paid')
        /// </summary>
        public string PaymentDifferenceHandling { get; set; }

        public Guid? WriteoffAccountId { get; set; }
        public AccountAccount WriteoffAccount { get; set; }

        public ICollection<AccountMovePaymentRel> AccountMovePaymentRels { get; set; } = new List<AccountMovePaymentRel>();

        public ICollection<SaleOrderPaymentRel> SaleOrderPaymentRels { get; set; } = new List<SaleOrderPaymentRel>();

        public ICollection<ServiceCardOrderPaymentRel> CardOrderPaymentRels { get; set; } = new List<ServiceCardOrderPaymentRel>();

        public ICollection<SaleOrderLinePaymentRel> SaleOrderLinePaymentRels { get; set; } = new List<SaleOrderLinePaymentRel>();

        [NotMapped]
        public AccountAccount DestinationAccount { get; set; }
    }
}
