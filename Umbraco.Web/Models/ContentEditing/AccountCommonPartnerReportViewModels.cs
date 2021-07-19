using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    /// <summary>
    /// form search báo cáo công nợ
    /// </summary>
    public class AccountCommonPartnerReportSearch
    {
        public AccountCommonPartnerReportSearch()
        {
            ResultSelection = "customer";
            //Display = "all";
            Display = "not_zero";
        }
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// Đối tác
        /// </summary>
        public Guid? PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public string ResultSelection { get; set; }

        public string Display { get; set; }

        public string Search { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class AccountCommonPartnerReportSearchV2
    {
        public AccountCommonPartnerReportSearchV2()
        {
            ResultSelection = "customer_supplier";
        }
        /// <summary>
        /// Từ ngày
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// Đến ngày
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Đối tác
        /// </summary>
        public IEnumerable<Guid> PartnerIds { get; set; } = new List<Guid>();

        /// <summary>
        /// 1.customer : force Customer
        /// 2.supplier : force Supplier
        /// 3.customer_supplier : All customer and supplier
        /// </summary>
        public string ResultSelection { get; set; }

        /// <summary>
        /// Force company
        /// </summary>
        public Guid? CompanyId { get; set; }
    }

    public class AccountCommonPartnerReportSearchV2Result
    {
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal InitialBalance { get; set; }
    }

    public class AccountCommonPartnerReportItem
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

        public string ResultSelection { get; set; }
    }

    public class AccountCommonPartnerReport
    {
        /// <summary>
        /// khach hang
        /// </summary>
        public Guid PartnerId { get; set; }
        /// <summary>
        /// Toong tien da mua dv
        /// </summary>
        public decimal Debit { get; set; }
        /// <summary>
        /// so tien da tra
        /// </summary>
        public decimal Credit { get; set; }

        /// <summary>
        /// con no
        /// </summary>
        public decimal InitialBalance { get; set; }

        public int CountSaleOrder { get; set; }

    }

    public class AccountCommonPartnerReportItemDetail
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

    public class ReportPartnerDebitReq
    {

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Guid? PartnerId { get; set; }

        public string Search { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class ReportPartnerDebitRes
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

    public class ReportPartnerDebitDetailReq
    {

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public Guid? PartnerId { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class ReportPartnerDebitDetailRes
    {
        public DateTime? Date { get; set; }
        public string InvoiceOrigin { get; set; }
        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal End { get; set; }

    }
}
