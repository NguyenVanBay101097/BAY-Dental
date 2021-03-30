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
        /// Phần trăm giảm giá
        /// </summary>
        public int DiscountPercent { get; set; }

        /// <summary>
        /// Tổng số tiền trên 1 lần thanh toán
        /// </summary>
        public int Amount { get; set; }

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
