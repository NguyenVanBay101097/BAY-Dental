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
            AccountType = "other";
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
        ///    THUCN/năm/sequence
        ///    CHIHH/năm/sequence
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// thu: phiếu thu
        /// chi: phiếu chi
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// commission
        /// customer_debt
        /// other
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Kèm theo, bỏ ko xài
        /// </summary>
        public string Communication { get; set; }

        /// <summary>
        /// Lý do nộp
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Người nộp tiền/Người nhận tiền, bỏ ko xài
        /// </summary>
        public string PayerReceiver { get; set; }

        /// <summary>
        /// Địa chỉ, bỏ ko xài
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Loại thu/Loại chi
        /// </summary>
        public Guid? LoaiThuChiId { get; set; }
        public LoaiThuChi LoaiThuChi { get; set; }

        /// <summary>
        /// Không sử dụng nữa
        /// </summary>
        public ICollection<AccountMoveLine> MoveLines { get; set; } = new List<AccountMoveLine>();

        /// <summary>
        /// customer
        /// supplier
        /// employee
        /// agent
        /// </summary>
        public string PartnerType { get; set; }

        public Guid? PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid? AgentId { get; set; }
        public Agent Agent { get; set; }

        public Guid? AccountId { get; set; }
        public AccountAccount Account { get; set; }

        /// <summary>
        /// hạch toán
        /// </summary>
        public bool IsAccounting { get; set; }
    }
}
