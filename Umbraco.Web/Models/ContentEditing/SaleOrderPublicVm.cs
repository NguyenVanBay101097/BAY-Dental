using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPublicFilter
    {
        public Guid PartnerId { get; set; }
    }

    public class SaleOrderPublic
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày điều trị
        /// </summary>
        public DateTime DateOrder { get; set; }

        /// <summary>
        /// Tong tien
        /// </summary>
        public decimal? AmountTotal { get; set; }
       
        /// <summary>
        /// Con lai
        /// </summary>
        public decimal? Residual { get; set; }

        /// <summary>
        /// Thanh toan
        /// </summary>
        public decimal? TotalPaid { get; set; }

        /// <summary>
        /// Tong giam
        /// </summary>
        public decimal DiscountTotal { get; set; }

        /// <summary>
        /// Trạng thái
        /// draft: Quotation
        /// sale: Sales Order
        /// done: Locked
        /// cancel: Cancelled
        /// </summary>
        public string State { get; set; }
    }
}
