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
        /// Lý do nộp
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Loại thu/Loại chi
        /// </summary>
        public Guid? LoaiThuChiId { get; set; }

        public string PartnerType { get; set; }

        public Guid? PartnerId { get; set; }
    }

    public class CustomerDebtSave
    {
        public CustomerDebtSave()
        {
            Type = "thu";
        }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }

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
        /// Lý do nộp
        /// </summary>
        public string Reason { get; set; }

        public Guid? PartnerId { get; set; }
    }

    public class CommissionAgentSave
    {
        public CommissionAgentSave()
        {
            Type = "chi";
        }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sổ nhật ký: tiền mặt, ngân hàng
        /// </summary>
        public Guid JournalId { get; set; }

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
        /// Lý do nộp
        /// </summary>
        public string Reason { get; set; }

        public Guid? PartnerId { get; set; }
    }
}
