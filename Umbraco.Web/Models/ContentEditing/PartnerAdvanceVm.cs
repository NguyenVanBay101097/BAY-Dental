using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class PartnerAdvanceBasic
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public string JournalName { get; set; }

        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tạm ứng
        /// </summary>
        public string Type { get; set; }

        public decimal Amount
        {
            get;
            set;
        }

        public string State { get; set; }
    }

    public class PartnerAdvancePaged
    {
        public PartnerAdvancePaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }
    }

    public class PartnerAdvanceSummaryFilter
    {
        public string Type { get; set; }
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }

    public class PartnerAdvanceGetSummary
    {
        public string Type { get; set; }

        public decimal AmountTotal { get; set; }
    }

    public class PartnerAdvanceDefaultFilter
    {
        public string Type { get; set; }
        public Guid PartnerId { get; set; }
    }

    public class PartnerAdvancDefaultViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }

        public Guid CompanyId { get; set; }
        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tạm ứng
        /// </summary>
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public decimal AmounAdvanceTotal { get; set; }

        public string State { get; set; }

        public string Note { get; set; }
    }


    public class PartnerAdvanceSave
    {
        public DateTime Date { get; set; }

        public Guid JournalId { get; set; }

        public Guid CompanyId { get; set; }

        public Guid PartnerId { get; set; }
        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tạm ứng
        /// </summary>
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string State { get; set; }

        public string Note { get; set; }
    }

    public class PartnerAdvanceDisplay
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public Guid JournalId { get; set; }
        public AccountJournalSimple Journal { get; set; }

        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }

        public Guid CompanyId { get; set; }
        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tạm ứng
        /// </summary>
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string State { get; set; }

        public string Note { get; set; }
    }

    public class PartnerAdvancePrint
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Ngày tạm ứng
        /// </summary>
        public DateTime Date { get; set; }


        public string CreatedById { get; set; }

        public string UserName { get; set; }

        public string PartnerName { get; set; }

        public string JournalName { get; set; }

        public Guid CompanyId { get; set; }
        public CompanyPrintVM Company { get; set; }

        /// <summary>
        /// advance : đóng tạm ứng
        /// refund : hoàn tạm ứng
        /// </summary>
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string AmountText { get; set; }

        public string State { get; set; }

        public string Note { get; set; }
    }

}
