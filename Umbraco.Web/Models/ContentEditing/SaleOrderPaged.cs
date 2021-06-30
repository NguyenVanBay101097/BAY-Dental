using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPaged
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid? PartnerId { get; set; }

        public DateTime? DateOrderFrom { get; set; }

        public DateTime? DateOrderTo { get; set; }

        public string State { get; set; }

        public bool? IsQuotation { get; set; }

        public Guid? Id { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// Interval Nbr
        /// </summary>
        public int? OverIntervalNbr { get; set; }

        /// <summary>
        /// Over interval: 'month'
        /// </summary>
        public string OverInterval { get; set; }
    }

    public class ApplyDiscountRequest
    {
        /// <summary>
        /// SaleOrderId
        /// </summary>
        public Guid Id { get; set; }

        public decimal Amount { get; set; }
    }
}
