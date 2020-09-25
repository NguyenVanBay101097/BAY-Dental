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

        /// <summary>
        /// Ngày bán
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Người bán
        /// </summary>
        public string UserId { get; set; }

        public Guid CompanyId { get; set; }

        public decimal? AmountRefund { get; set; }

        public IEnumerable<ServiceCardOrderLineSave> OrderLines { get; set; } = new List<ServiceCardOrderLineSave>();
    }
}
