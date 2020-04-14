using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PurchaseOrderLine: BaseEntity
    {
        public PurchaseOrderLine()
        {
            QtyInvoiced = 0;
            Discount = 0;
        }

        public string Name { get; set; }
        public int? Sequence { get; set; }
        public decimal ProductQty { get; set; }
        public decimal? ProductUOMQty { get; set; }
        public Guid? ProductUOMId { get; set; }
        public UoM ProductUOM { get; set; }
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
        public decimal? PriceSubtotal { get; set; }
        public decimal? PriceTotal { get; set; }
        public decimal? PriceTax { get; set; }
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
        public decimal PriceUnit { get; set; }
        public Guid OrderId { get; set; }
        public PurchaseOrder Order { get; set; }
        public string State { get; set; }
        public DateTime? DatePlanned { get; set; }
        public decimal? Discount { get; set; }
        public ICollection<AccountInvoiceLine> InvoiceLines { get; set; } = new List<AccountInvoiceLine>();
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        public decimal? QtyInvoiced { get; set; }

        public ICollection<AccountMoveLine> MoveLines { get; set; } = new List<AccountMoveLine>();
    }
}
