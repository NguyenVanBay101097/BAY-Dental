using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PaymentQuotation : BaseEntity
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

        /// <summary>
        /// Bảng báo giá
        /// </summary>
        public Quotation Quotation { get; set; }
        public Guid QuotationId { get; set; }
    }
}
