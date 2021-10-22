using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class PhieuThuChiPrintTemplate
    {
        public CompanyPrintTemplate Company { get; set; }

        public PartnerPrintTemplate Partner { get; set; }

        public AgentPrintTemplate Agent { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime DateCreated { get; set; }

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

        public string AmountText
        {
            get
            {

                return AmountToText.amount_to_text(Amount);
            }
            set { }
        }

        public string CreatedByName { get; set; }

        public string PartnerName { get; set; }

        public ApplicationUserPrintTemplate User { get; set; }
    }

    public class AgentPrintTemplate
    {
        public string Name { get; set; }

        public string Gender { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }
    }
}
