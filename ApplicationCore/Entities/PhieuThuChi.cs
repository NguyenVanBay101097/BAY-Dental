using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class PhieuThuChi: BaseEntity
    {
        public PhieuThuChi()
        {
            State = "draft";
            Date = DateTime.Today;
        }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }
        public AccountJournal Journal { get; set; }

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
        public LoaiThuChi LoaiThuChi { get; set; }

        /// <summary>
        /// Không sử dụng nữa
        /// </summary>
        public ICollection<AccountMoveLine> MoveLines { get; set; } = new List<AccountMoveLine>();

        public string PartnerType { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }
    }
}
