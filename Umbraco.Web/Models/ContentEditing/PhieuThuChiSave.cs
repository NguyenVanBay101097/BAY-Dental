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
        /// Nội dung
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// commission
        /// customer_debt
        /// other:
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Loại thu/Loại chi nếu AccountType = other
        /// </summary>
        public Guid? LoaiThuChiId { get; set; }

        /// <summary>
        /// Bắt buộc có giá trị khi AccountType = commission
        /// </summary>
        public Guid? AgentId { get; set; }

        /// <summary>
        /// Thu chi ngoài
        /// </summary>
        public string PartnerType { get; set; }

        /// <summary>
        /// Thu chi ngoài, customer_debt, commission
        /// </summary>
        public Guid? PartnerId { get; set; }

        public bool IsAccounting { get; set; }


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

        public Guid? AgentId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
