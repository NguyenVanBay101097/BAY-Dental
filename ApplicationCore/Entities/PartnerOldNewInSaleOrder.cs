using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PartnerOldNewInSaleOrder
    {
        public DateTime DateOrder { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public decimal? TotalPaid { get; set; }
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }
        public Guid SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }
        public bool IsNew { get; set; }
    }
}
