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

    public class SaleOrderSimple
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
