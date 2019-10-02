using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Chi tiết bán dịch vụ
    /// </summary>
    public class SaleOrderLine: BaseEntity
    {
        public SaleOrderLine()
        {
            State = "draft";
        }

        public decimal PriceUnit { get; set; }

        public decimal ProductUOMQty { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public Guid? OrderPartnerId { get; set; }
        public Partner OrderPartner { get; set; }

        public Guid OrderId { get; set; }
        public SaleOrder Order { get; set; }

        /// <summary>
        /// %
        /// </summary>
        public decimal Discount { get; set; }

        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        public decimal PriceSubTotal { get; set; }

        public decimal PriceTax { get; set; }

        public decimal PriceTotal { get; set; }

        public string SalesmanId { get; set; }
        public ApplicationUser Salesman { get; set; }

        public string Note { get; set; }
    }
}
