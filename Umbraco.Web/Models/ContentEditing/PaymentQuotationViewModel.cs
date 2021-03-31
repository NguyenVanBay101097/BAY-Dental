using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PaymentQuotationBasic
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
        public int DiscountPercent { get; set; }

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
