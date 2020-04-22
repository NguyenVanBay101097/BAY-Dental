using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class ServiceCardOrderSave
    {
        /// <summary>
        /// Khách hàng sẽ ghi công nợ
        /// </summary>
        public Guid PartnerId { get; set; }

        public Guid CardTypeId { get; set; }

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

        public decimal? PriceUnit { get; set; }

        public decimal? Quantity { get; set; }

        public string GenerationType { get; set; }
    }
}
