using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiDisplay
    {
        public PhieuThuChiDisplay()
        {
            Date = DateTime.Today;
            State = "draft";
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public CompanySimple Company { get; set; }

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
        /// Loại thu/Loại chi
        /// </summary>
        public Guid LoaiThuChiId { get; set; }
        public LoaiThuChiSimple LoaiThuChi { get; set; }

        public string PartnerType { get; set; }

        public PartnerSimple Partner { get; set; }
    }

    public class PhieuThuChiDefaultGet
    {
        public string Type { get; set; }
    }
}
