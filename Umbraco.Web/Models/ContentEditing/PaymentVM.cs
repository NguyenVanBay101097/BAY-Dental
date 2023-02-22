using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderCustomerDebtPaymentReq
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Guid OrderId { get; set; }
        //public Guid CompanyId { get; set; }
        /// <summary>
        /// state = state payment
        /// </summary>
        //public string State { get; set; }

        public List<SaleOrderPaymentJournalLineSave> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineSave>();

        public List<SaleOrderPaymentHistoryLineSave> Lines { get; set; } = new List<SaleOrderPaymentHistoryLineSave>();

        public string Note { get; set; }

        //thông tin thanh toán công nợ
        public bool IsDebtPayment { get; set; }

        public Guid? DebtJournalId { get; set; }

        public decimal DebtAmount { get; set; }

        public string DebtNote { get; set; }

        //public Guid PartnerId { get; set; }
    }
}
