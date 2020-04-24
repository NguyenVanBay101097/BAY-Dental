using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Lưu lại lịch sử apply service card card
    /// </summary>
    public class SaleOrderServiceCardCardRel: BaseEntity
    {
        public Guid SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }

        public Guid CardId { get; set; }
        public ServiceCardCard Card { get; set; }

        public decimal? Amount { get; set; }
    }
}
