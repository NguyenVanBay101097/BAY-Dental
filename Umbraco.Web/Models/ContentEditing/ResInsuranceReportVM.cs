using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class InsuranceDebtFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? InsuranceId { get; set; }
    }

    public class InsuranceDebtDetailFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid PaymentId { get; set; }
    }

    public class InsuranceDebtDetailItem
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class InsuranceHistoryInComeFilter
    {
        public InsuranceHistoryInComeFilter()
        {
            Limit = 20;
        }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? InsuranceId { get; set; }
    }

    public class InsuranceHistoryInComeDetailFilter
    {
        public Guid? PaymentId { get; set; }
    }

    public class InsuranceHistoryInCome
    {
        public Guid Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public string JournalName { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public string Communication { get; set; }

       
    }

    public class InsuranceHistoryInComeDetailLine
    {
        public Guid MoveId { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string PartnerRef { get; set; }

        public DateTime? Date { get; set; }

        public decimal AmountTotal { get; set; }

        public Guid? PaymentId { get; set; }

        public string Communication { get; set; }

    }


    public class InsuranceHistoryInComeDetailItem
    {
        public Guid Id { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal AmountTotal { get; set; }

        public string Communication { get; set; }

        public string State { get; set; }     

        public IEnumerable<InsuranceHistoryInComeDetailLine> Lines { get; set; } = new List<InsuranceHistoryInComeDetailLine>();
    }

    public class InsuranceDebtReport
    {
        public Guid PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string PartnerRef { get; set; }

        public DateTime? Date { get; set; }

        public decimal AmountTotal { get; set; }

        public string Origin { get; set; }

        public string Communication { get; set; }

        public Guid MoveId { get; set; }
        public string MoveType { get; set; }

        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Phieu thanh toan
        /// </summary>
        public Guid? PaymentId { get; set; }
    }

    /// <summary>
    /// filter báo cáo công nợ
    /// </summary>
    public class InsuranceReportFilter
    {
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        /// <summary>
        /// Chi nhanh
        /// </summary>
        public Guid? CompanyId { get; set; }
    }

    public class InsuranceReportPrint
    {
        public IEnumerable<InsuranceReportItem> Data { get; set; } = new List<InsuranceReportItem>();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
    }

    public class InsuranceReportItem
    {
        public Guid InsuranceId { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string PartnerRef { get; set; }

        public string PartnerPhone { get; set; }

        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal End { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }

    }

    public class InsuranceReportDetailFilter
    {
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? DateTo { get; set; }

        public Guid PartnerId { get; set; }

        /// <summary>
        /// Chi nhanh
        /// </summary>
        public Guid? CompanyId { get; set; }
    }

    public class InsuranceReportDetailItem
    {
        public DateTime? Date { get; set; }

        public string PaymentName { get; set; }

        public string Name { get; set; }

        public string Ref { get; set; }

        public string MoveName { get; set; }

        /// <summary>
        /// Dư ban đầu
        /// </summary>
        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        /// <summary>
        /// Dư cuối
        /// </summary>
        public decimal End { get; set; }

        public string PaymentCommunication { get; set; }

        public string PaymentPartnerName { get; set; }
    }

    public class ReportInsuranceDebitExcel : InsuranceReportItem
    {
        public IEnumerable<InsuranceReportDetailItem> Lines { get; set; } = new List<InsuranceReportDetailItem>();
    }
}
