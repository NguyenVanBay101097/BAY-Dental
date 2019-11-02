using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleReport
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public decimal ProductUOMQty { get; set; }
        public Guid? ProductUOMId { get; set; }
        public UoM ProductUOM { get; set; }
        public decimal? QtyToInvoice { get; set; }
        public decimal? QtyInvoiced { get; set; }
        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal PriceSubTotal { get; set; }
        public decimal PriceTotal { get; set; }
        public Guid? CategId { get; set; }
        public ProductCategory Categ { get; set; }
        public int Nbr { get; set; }
        public string State { get; set; }
    }
}
