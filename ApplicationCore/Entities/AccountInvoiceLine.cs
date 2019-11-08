using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class AccountInvoiceLine: BaseEntity
    {
        public string Name { get; set; }

        public string Origin { get; set; }

        public int Sequence { get; set; }

        public Guid? InvoiceId { get; set; }
        public AccountInvoice Invoice { get; set; }

        public Guid? UoMId { get; set; }
        public UoM UoM { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid AccountId { get; set; }
        public AccountAccount Account { get; set; }

        public decimal PriceUnit { get; set; }
        public decimal PriceSubTotal { get; set; }
        public decimal PriceSubTotalSigned { get; set; }
        public decimal Discount { get; set; }
        public decimal Quantity { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Bác sĩ, employee == true
        /// </summary>
        public Guid? EmployeeId { get; set; }
        public Partner Employee { get; set; }

        /// <summary>
        /// Bác sĩ, employee == true
        /// </summary>
        public Guid? ToothCategoryId { get; set; }
        public ToothCategory ToothCategory { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Chẩn đoán
        /// </summary>
        public string Diagnostic { get; set; }

        public Guid? ToothId { get; set; }
        public Tooth Tooth { get; set; }

        public ICollection<AccountInvoiceLineToothRel> AccountInvoiceLineToothRels { get; set; } = new List<AccountInvoiceLineToothRel>();

        public ICollection<SaleOrderLineInvoiceRel> SaleLines { get; set; } = new List<SaleOrderLineInvoiceRel>();

        public Guid? LaboLineId { get; set; }
        public LaboOrderLine LaboLine { get; set; }

        public Guid? PurchaseLineId { get; set; }
        public PurchaseOrderLine PurchaseLine { get; set; }
    }
}
