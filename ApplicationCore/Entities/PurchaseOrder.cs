using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PurchaseOrder: BaseEntity
    {
        public PurchaseOrder()
        {
            Name = "/";
            State = "draft";
            DateOrder = DateTime.Now;
            Type = "order";
        }
        public string Name { get; set; }

        public string State { get; set; }

        public string PartnerRef { get; set; }

        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public DateTime DateOrder { get; set; }

        public DateTime? DateApprove { get; set; }

        public Guid PickingTypeId { get; set; }
        public StockPickingType PickingType { get; set; }

        public decimal? AmountTotal { get; set; }

        public decimal? AmountUntaxed { get; set; }

        public decimal? AmountTax { get; set; }

        public string Origin { get; set; }

        public ICollection<PurchaseOrderLine> OrderLines { get; set; } = new List<PurchaseOrderLine>();

        public DateTime? DatePlanned { get; set; }

        public string Notes { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string InvoiceStatus { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Dung de phan loai: don mua hang hay don tra hang
        /// </summary>
        public string Type { get; set; }

        public Guid? RefundOrderId { get; set; }
        public PurchaseOrder RefundOrder { get; set; }
    }
}
