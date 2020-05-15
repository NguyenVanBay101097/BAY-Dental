using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrderLine: BaseEntity
    {
        public ServiceCardOrderLine()
        {
            DiscountType = "percentage";
            Discount = 0;
            DiscountFixed = 0;
        }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string State { get; set; }

        public Guid? OrderPartnerId { get; set; }
        public Partner OrderPartner { get; set; }

        public Guid OrderId { get; set; }
        public ServiceCardOrder Order { get; set; }

        public decimal Discount { get; set; }

        public Guid CardTypeId { get; set; }
        public ServiceCardType CardType { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTotal { get; set; }

        public string SalesmanId { get; set; }
        public ApplicationUser Salesman { get; set; }

        public int? Sequence { get; set; }

        public string DiscountType { get; set; }

        public decimal? DiscountFixed { get; set; }

        public ICollection<ServiceCardOrderLineInvoiceRel> OrderLineInvoiceRels { get; set; } = new List<ServiceCardOrderLineInvoiceRel>();

        public ICollection<ServiceCardCard> Cards { get; set; } = new List<ServiceCardCard>();
    }
}
