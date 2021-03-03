using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderViewModel
    {
        public Guid Id { get; set; }

        public DateTime DateOrder { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerDisplayName { get; set; }

        public decimal? AmountTotal { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal? Residual { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Là phiếu tư vấn
        /// </summary>
        public bool? IsQuotation { get; set; }
    }

    public class ActionDonePar
    {
        public IEnumerable<Guid> Ids { get; set; }
    }

}
