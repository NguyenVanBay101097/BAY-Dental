using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class LaboOrderLine: BaseEntity
    {
        public LaboOrderLine()
        {
            Sequence = 10;
        }

        public string Name { get; set; }

        public int? Sequence { get; set; }

        public decimal ProductQty { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? CustomerId { get; set; }
        public Partner Customer { get; set; }

        /// <summary>
        /// Màu sắc
        /// </summary>
        public string Color { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal PriceSubtotal { get; set; }

        public decimal PriceTotal { get; set; }

        public Guid OrderId { get; set; }
        public LaboOrder Order { get; set; }

        /// <summary>
        /// Mã bảo hành
        /// </summary>
        public string WarrantyCode { get; set; }

        /// <summary>
        /// Hạn bảo hành
        /// </summary>
        public DateTime? WarrantyPeriod { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public string Note { get; set; }

        public Guid? ToothCategoryId { get; set; }
        public ToothCategory ToothCategory { get; set; }

        public ICollection<LaboOrderLineToothRel> LaboOrderLineToothRels { get; set; } = new List<LaboOrderLineToothRel>();

        public ICollection<AccountInvoiceLine> InvoiceLines { get; set; } = new List<AccountInvoiceLine>();

        public ICollection<AccountMoveLine> MoveLines { get; set; } = new List<AccountMoveLine>();

        public decimal QtyInvoiced { get; set; }

        public string State { get; set; }

        public Guid? SaleOrderLineId { get; set; }
        
        public SaleOrderLine SaleOrderLine { get; set; }
        /// <summary>
        /// đã nhận hay chưa
        /// </summary>
        public bool IsReceived { get; set; }
        /// <summary>
        /// ngày nhận thực tế
        /// </summary>
        public DateTime? ReceivedDate { get; set; }

    }
}
