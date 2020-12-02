using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiPrintVM
    {
        public string CompanyName { get; set; }

        public string CompanyLogo { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public string JournalName { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Kèm theo
        /// </summary>
        public string Communication { get; set; }

        /// <summary>
        /// Lý do nộp
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Người nộp tiền/Người nhận tiền
        /// </summary>
        public string PayerReceiver { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        public string LoaiThuChiName { get; set; }

        public string AmountText { get; set; }
    }
}
