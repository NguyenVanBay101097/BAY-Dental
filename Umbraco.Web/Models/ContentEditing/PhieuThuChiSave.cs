using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiSave
    {
        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        /// <summary>
        /// draft: Nháp
        /// posted: Đã vào sổ
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Mã THU/năm/sequence
        ///    CHI/năm/sequence
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// thu: phiếu thu
        /// chi: phiếu chi
        /// </summary>
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

        /// <summary>
        /// Loại thu/Loại chi
        /// </summary>
        public Guid LoaiThuChiId { get; set; }
        public LoaiThuChiSimple LoaiThuChi { get; set; }
    }
}
