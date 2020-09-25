using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ServiceCardOrder: BaseEntity
    {
        public ServiceCardOrder()
        {
            State = "draft";
            DateOrder = DateTime.Now;
        }

        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Khách hàng sẽ ghi công nợ
        /// </summary>
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        /// <summary>
        /// Ngày bán
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Người bán
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string State { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public decimal? AmountTotal { get; set; }

        public decimal? AmountResidual { get; set; }

        /// <summary>
        /// tiền hoàn trả khách hàng
        /// </summary>
        public decimal? AmountRefund { get; set; }

        public ICollection<ServiceCardOrderLine> OrderLines { get; set; } = new List<ServiceCardOrderLine>();
    }
}
