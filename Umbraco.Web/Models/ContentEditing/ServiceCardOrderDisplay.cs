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

        public Guid CardTypeId { get; set; }
        public ServiceCardTypeBasic CardType { get; set; }

        /// <summary>
        /// Ngày bán
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Ngày cấp thẻ
        /// </summary>
        public DateTime? ActivatedDate { get; set; }

        /// <summary>
        /// Người bán
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUserSimple User { get; set; }

        public decimal? PriceUnit { get; set; }

        public decimal? Quantity { get; set; }

        public string GenerationType { get; set; }

        public decimal? AmountTotal { get; set; }
    }
}
