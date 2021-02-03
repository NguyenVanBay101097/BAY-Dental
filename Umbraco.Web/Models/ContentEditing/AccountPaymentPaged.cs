using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountPaymentPaged
    {
        public AccountPaymentPaged()
        {
            PhieuThuChi = false;
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        /// <summary>
        /// customer 
        /// supplier
        /// employee
        /// </summary>
        public string PartnerType { get; set; }

        /// <summary>
        /// posted: xác nhận
        /// draft: nháp
        /// </summary>
        public string State { get; set; }

        public DateTime? PaymentDateFrom { get; set; }
        public DateTime? PaymentDateTo { get; set; }

        public Guid? SaleOrderId { get; set; }
        public Guid? PartnerId { get; set; }

        public bool PhieuThuChi { get; set; }
        public string PaymentType { get; set; }
        /// <summary>
        /// cash: tiền mặt
        /// bank: ngân hàng
        /// cash_bank: tổng sổ quỹ
        /// </summary>
        public string ResultSelection { get; set; }

    }
}
