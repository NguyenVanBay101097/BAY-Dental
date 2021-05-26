using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public string PartnerName { get; set; }

        public decimal? AmountTotal { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Quotation
        /// sale: Sales Order
        /// done: Locked
        /// cancel: Cancelled
        /// </summary>
        public string State { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public decimal? Residual { get; set; }
    }

    public class SaleOrderSmsBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime? DateDone { get; set; }

        /// <summary>
        /// Khách hàng
        /// </summary>
        public string PartnerName { get; set; }
        public string PartnerPhone { get; set; }
        public Guid PartnerId { get; set; }

        public decimal? AmountTotal { get; set; }

        public string Name { get; set; }

        public string SaleOrderLineName { get; set; }
    }
}
