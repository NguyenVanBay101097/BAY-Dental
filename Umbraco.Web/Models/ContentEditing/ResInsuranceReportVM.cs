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

    public class InsuranceDebtReport
    {
        public string PartnerName { get; set; }
        public DateTime? Date { get; set; }

        public decimal AmountTotal { get; set; }

        public string Origin { get; set; }

        public Guid MoveId { get; set; }
        public string MoveType { get; set; }
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

    public class InsuranceReportItem
    {
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
    }
}
