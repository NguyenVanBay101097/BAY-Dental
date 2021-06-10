using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class SaleOrderPaymentPaged
    {
        public SaleOrderPaymentPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public Guid? SaleOrderId { get; set; }

    }





    public class SaleOrderPaymentBasic
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public string Note { get; set; }

        public IEnumerable<AccountPaymentBasic> Payments { get; set; } = new List<AccountPaymentBasic>();

        /// <summary>
        /// draft : nháp
        /// posted : đã thanh toán
        /// cancel : hủy
        /// </summary>
        public string State { get; set; }
    }

    public class SaleOrderPaymentDisplay
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<SaleOrderPaymentJournalLineDisplay> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineDisplay>();
        public IEnumerable<SaleOrderPaymentHistoryLineDisplay> Lines { get; set; } = new List<SaleOrderPaymentHistoryLineDisplay>();

        public string Note { get; set; }

        /// <summary>
        /// state = state payment
        /// </summary>
        public string State { get; set; }
    }

    public class SaleOrderPaymentSave
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Guid OrderId { get; set; }
        public Guid CompanyId { get; set; }

        public IEnumerable<SaleOrderPaymentJournalLineSave> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineSave>();
        public IEnumerable<SaleOrderPaymentHistoryLineSave> Lines { get; set; } = new List<SaleOrderPaymentHistoryLineSave>();

        public string Note { get; set; }

        /// <summary>
        /// state = state payment
        /// </summary>
        public string State { get; set; }
    }

    public class RegisterSaleOrderPayment
    {
        public RegisterSaleOrderPayment()
        {
            Date = DateTime.Today;
            State = "draft";
        }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Guid OrderId { get; set; }
        public Guid CompanyId { get; set; }

        public IEnumerable<SaleOrderPaymentJournalLineDisplay> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineDisplay>();
        public IEnumerable<RegisterSaleOrderPaymentHistoryLine> Lines { get; set; } = new List<RegisterSaleOrderPaymentHistoryLine>();

        public string Note { get; set; }

        /// <summary>
        /// state = state payment
        /// </summary>
        public string State { get; set; }
    }

    public class HistoryPartnerAdvanceFilter
    {
        public HistoryPartnerAdvanceFilter()
        {
            Limit = 20;
        }
        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? PartnerId { get; set; }

    }


    public class HistoryPartnerAdvanceResult
    {
        public string PaymentName { get; set; }
        public DateTime PaymentDate { get; set; }

        public decimal PaymentAmount { get; set; }

        public IEnumerable<SaleOrderSimple> Orders { get; set; }
    }

    public class SaleOrderPaymentPrintVM
    {
        public Guid Id { get; set; }
        public CompanyPrintVM Company { get; set; }
        public PartnerPrintVM OrderPartner { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<SaleOrderPaymentJournalLineDisplay> JournalLines { get; set; } = new List<SaleOrderPaymentJournalLineDisplay>();

        public string Note { get; set; }
        public ApplicationUserSimple User { get; set; }
    }
}
