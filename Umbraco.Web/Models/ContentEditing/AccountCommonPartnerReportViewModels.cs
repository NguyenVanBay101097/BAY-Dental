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

    public class SampleDataAgeFilter
    {

        public string Name { get; set; }
        public int? AgeFrom { get; set; }

        public int? AgeTo { get; set; }
    }

    

    public class PartnerReportOverviewItem
    {
        public Guid PartnerId { get; set; }

        public string PartnerGender { get; set; }
        /// <summary>
        /// Năm sinh
        /// </summary>
        public int? Age { get; set; }

        public Guid? PartnerSourceId { get; set; }

        public string PartnerSourceName { get; set; }


        /// <summary>
        /// thành phố
        /// </summary>
        public string CityName { get; set; }

        public string CityCode { get; set; }

        /// <summary>
        /// quận/ huyện
        /// </summary>
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }

        /// <summary>
        /// phường/xã
        /// </summary>
        public string WardName { get; set; }
        public string WardCode { get; set; }

        public string OrderState { get; set; }

        public decimal TotalService { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalRevenueExpect { get; set; }

        public decimal TotalDebt { get; set; }
    }

    public class AccountCommonPartnerReportOverview
    {
        public decimal TotalPartner { get; set; }

        public decimal TotalService { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal TotalRevenueExpect { get; set; }

        public decimal TotalDebt { get; set; }
    }

    public class PartnerReportSourceOverview
    {
        public Guid? PartnerSourceId { get; set; }

        public string PartnerSourceName { get; set; }

        public int TotalPartner { get; set; }

    }

    public class PartnerGenderReportOverview
    {
        public IEnumerable<string> XAxisChart { get; set; }
        public IEnumerable<string> LegendChart { get; set; }
        public List<PartnerGenderItemReportOverview> PartnerGenderItems { get; set; } = new List<PartnerGenderItemReportOverview>();



    }

    public class PartnerGenderItemReportOverview
    {
        public string PartnerGender { get; set; }
        public double PartnerGenderPercent { get; set; }
        public IEnumerable<double> Percent { get; set; }
        public IEnumerable<int> Count { get; set; }
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
        public string HrJobName { get; set; }

        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal End { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }

        public string ResultSelection { get; set; }
    }

    public class AccountCommonPartnerReportPrint
    {
        public IEnumerable<AccountCommonPartnerReportItem> Data { get; set; } = new List<AccountCommonPartnerReportItem>();
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
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

    public class GetPartnerForCityReportOverview
    {
        public string CityName { get; set; }

        public string CityCode { get; set; }

        public int Count { get; set; }

        public IEnumerable<GetPartnerForDistrictReportOverview> Districts { get; set; } = new List<GetPartnerForDistrictReportOverview>();

    }

    public class GetPartnerForDistrictReportOverview
    {
        public string DistrictName { get; set; }

        public string DistrictCode { get; set; }

        public int Count { get; set; }

        public IEnumerable<GetPartnerForWardReportOverview> Wards { get; set; } = new List<GetPartnerForWardReportOverview>();
    }

    public class GetPartnerForWardReportOverview
    {
        public string WardName { get; set; }

        public string WardCode { get; set; }

        public int Count { get; set; }

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

    public class ReportPartnerDebitSummaryRes
    {
        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }
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
        public Guid PartnerId { get; set; }
        public DateTime? Date { get; set; }
        public string InvoiceOrigin { get; set; }
        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal End { get; set; }
        public string Ref { get; set; }

    }

    public class ReportPartnerDebitPrint : ReportPartnerDebitRes
    {
        public IEnumerable<ReportPartnerDebitDetailRes> Lines { get; set; } = new List<ReportPartnerDebitDetailRes>();
    }

    public class ReportPartnerDebitExcel : ReportPartnerDebitRes
    {
        public IEnumerable<ReportPartnerDebitDetailRes> Lines { get; set; } = new List<ReportPartnerDebitDetailRes>();
    }

    public class ReportPartnerDebitPrintVM
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
        public IEnumerable<ReportPartnerDebitPrint> ReportPartnerDebitLines { get; set; } = new List<ReportPartnerDebitPrint>();
    }

    public class ReportPartnerAdvanceFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class ReportPartnerAdvance
    {
        public Guid PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string PartnerPhone { get; set; }

        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Refund { get; set; }

        public decimal End { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class ReportPartnerAdvanceDetailFilter
    {

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? PartnerId { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class ReportPartnerAdvanceDetail
    {
        public DateTime? Date { get; set; }
        public string InvoiceOrigin { get; set; }
        public decimal Begin { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal End { get; set; }
        public string Name { get; set; }

        public Guid? PartnerId { get; set; }
    }

    public class ReportPartnerAdvancePrintVM
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public CompanyPrintVM Company { get; set; }
        public ApplicationUserSimple User { get; set; }
        public IEnumerable<ReportPartnerAdvance> ReportPartnerAdvances { get; set; } = new List<ReportPartnerAdvance>();
    }
}
