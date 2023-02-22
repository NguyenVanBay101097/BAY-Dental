using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        /// <summary>
        /// Khách hàng sẽ ghi công nợ
        /// </summary>
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        /// <summary>
        /// Ngày bán
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Người bán
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public Guid CompanyId { get; set; }

        public IEnumerable<ServiceCardOrderLineDisplay> OrderLines { get; set; } = new List<ServiceCardOrderLineDisplay>();

        public IEnumerable<ServiceCardOrderPaymentDisplay> Payments { get; set; } = new List<ServiceCardOrderPaymentDisplay>();

        public decimal? AmountTotal { get; set; }

        public decimal? AmountResidual { get; set; }

        public int CardCount { get; set; }
    }
}
