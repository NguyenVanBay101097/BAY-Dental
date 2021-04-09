using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountMoveLine: BaseEntity
    {
        public AccountMoveLine()
        {
            ExcludeFromInvoiceTab = false;
            Discount = 0;
            Quantity = 1;
            Name = "/";
            DiscountType = "percentage";
            DiscountFixed = 0;
            Reconciled = false;
            AmountResidual = 0;
        }

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
        public DateTime? DateMaturity { get; set; }

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

        public decimal? Discount { get; set; }

        public decimal? PriceUnit { get; set; }

        public ICollection<SaleOrderLineInvoice2Rel> SaleLineRels { get; set; } = new List<SaleOrderLineInvoice2Rel>();

        /// <summary>
        /// Technical field used to exclude some lines from the invoice_line_ids tab in the form view.
        /// </summary>
        public bool? ExcludeFromInvoiceTab { get; set; }

        /// <summary>
        /// related: Move.Name
        /// </summary>
        public string MoveName { get; set; }

        /// <summary>
        /// related: Move.State
        /// </summary>
        public string ParentState { get; set; }

        public string AccountInternalType { get; set; }

        public decimal? PriceSubtotal { get; set; }

        public decimal? PriceTotal { get; set; }

        public Guid? PurchaseLineId { get; set; }
        public PurchaseOrderLine PurchaseLine { get; set; }

        public Guid? LaboLineId { get; set; }
        public LaboOrderLine LaboLine { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }

        public ICollection<ServiceCardOrderLineInvoiceRel> CardOrderLineRels { get; set; } = new List<ServiceCardOrderLineInvoiceRel>();

        public Guid? PhieuThuChiId { get; set; }
        public PhieuThuChi PhieuThuChi { get; set; }

        public string SalesmanId { get; set; }
        public ApplicationUser Salesman { get; set; }

        public ICollection<CommissionSettlement> CommissionSettlements { get; set; } = new List<CommissionSettlement>();
    }
}
