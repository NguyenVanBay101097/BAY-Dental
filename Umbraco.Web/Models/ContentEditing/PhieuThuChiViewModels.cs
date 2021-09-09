﻿using ApplicationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PhieuThuChiPrintVM
    {
        public CompanyPrintVM Company { get; set; }

        public PartnerPrintVM Partner { get; set; }

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

        public string AmountText {
            get
            {
               
                return AmountToText.amount_to_text(Amount);
            } set { } 
        }

        public string CreatedByName { get; set; }

        public string PartnerName { get; set; }

        public ApplicationUserSimple User { get; set; }
    }

    public class PrintVM
    {
        public CompanyPrintVM Company { get; set; }

        public PartnerPrintVM Partner { get; set; }

        public AgentPrintVM Agent { get; set; }

        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime DateCreated { get; set; }

        public string JournalName { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string AccountType { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }


        /// <summary>
        /// Lý do nộp
        /// </summary>
        public string Reason { get; set; }


        public string AmountText
        {
            get
            {

                return AmountToText.amount_to_text(Amount);
            }
            set { }
        }

        public string CreatedByName { get; set; }

        public ApplicationUserSimple User { get; set; }
    }
}
