using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PaymentQuotationDisplay
    {
        /// <summary>
        /// Số thứ tự thanh toán
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Phần trăm / Tiền mặt
        /// </summary>
        public string DiscountPercentType { get; set; }

        /// <summary>
        /// Số tiền thanh toán / Sô %
        /// </summary>
        public decimal? Payment { get; set; }

        /// <summary>
        /// Tổng số tiền trên 1 lần thanh toán
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime? Date { get; set; }

        //public QuotationSimple Quotation { get; set; }
        //public Guid QuotationId { get; set; }
    }

    public class PaymentQuotationSave
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Số thứ tự thanh toán
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// Phần trăm giảm giá
        /// </summary>
        public string DiscountPercentType { get; set; }
        public string Payment { get; set; }
        /// <summary>
        /// Tổng số tiền trên 1 lần thanh toán
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public DateTime? Date { get; set; }
    }
}
