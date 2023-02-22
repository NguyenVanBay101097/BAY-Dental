using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class SaleOrderLineProductRequested: BaseEntity
    {
        //saleorderline
        public Guid SaleOrderLineId { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }
        //vật tư
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        //số lượng đã yêu cầu
        public decimal RequestedQuantity { get; set; }
    }
}
