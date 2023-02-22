using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class DotKhamStep : BaseEntity
    {
        public DotKhamStep()
        {
        }

        public DotKhamStep(DotKhamStep step)
        {
            Name = step.Name;
            ProductId = step.ProductId;
            SaleLineId = step.SaleLineId;
            SaleOrderId = step.SaleOrderId;
            Order = step.Order;
        }

        /// <summary>
        /// Công đoạn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dịch vụ
        /// </summary>
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        public Guid? SaleLineId { get; set; }
        public SaleOrderLine SaleLine { get; set; }

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        public bool IsDone { get; set; }

        /// <summary>
        /// Thứ tự
        /// </summary>
        public int? Order { get; set; }
    }
}
