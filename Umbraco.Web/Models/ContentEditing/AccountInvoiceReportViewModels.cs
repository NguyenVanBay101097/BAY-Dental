using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class RevenueReportQueryCommon
    {
        public RevenueReportQueryCommon(DateTime? dateFrom, DateTime? dateTo, Guid? companyId, string searchPartner = "")
        {
            this.DateFrom = dateFrom;
            this.DateTo = dateTo;
            this.CompanyId = companyId;
            this.SearchPartner = searchPartner;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public string SearchPartner { get; set; }
    }

    public class RevenueTimeReportPar
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class RevenueServiceReportPar
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class RevenueEmployeeReportPar
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// groupby: employee, assistant
        /// </summary>
        public string GroupBy { get; set; }
        public Guid? GroupById { get; set; }

    }


    public class RevenuePartnerReportPar
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public string Search { get; set; }

    }

    public class EmployeeAssistantKeyGroup
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
    }

    public class RevenueTimeReportDisplay
    {
        public DateTime? InvoiceDate { get; set; }
        public decimal PriceSubTotal { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class RevenueServiceReportDisplay
    {
        public string ProductName { get; set; }
        public Guid? ProductId { get; set; }
        public decimal PriceSubTotal { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class RevenueEmployeeReportDisplay
    {
        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        public decimal PriceSubTotal { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public string GroupBy { get; set; }
        public Guid ToDetailEmployeeId { get; set; }
    }

    public class RevenuePartnerReportDisplay
    {
        public string PartnerName { get; set; }
        public Guid? PartnerId { get; set; }
        public decimal PriceSubTotal { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public string Search { get; set; }
    }


    public class RevenueReportDetailPaged
    {
        public RevenueReportDetailPaged()
        {
            Limit = 20;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? AssistantId { get; set; }
        public Guid? PartnerId { get; set; }
    }

    public class RevenueReportDetailDisplay
    {
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceOrigin { get; set; }
        public Guid? PartnerId { get; set; }
        public string PartnerName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid? AssistantId { get; set; }
        public string AssistantName { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal PriceSubTotal { get; set; }
    }
  
    public class SumRevenueReportPar
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? PartnerId { get; set; }
    }


    public class RevenueTimeReportPrintVM
    {
        public DateTime InvoiceDate { get; set; }
        public decimal PriceSubTotal { get; set; }
        public IEnumerable<RevenueReportDetailDisplay> RevenueReportDetailPrints { get; set; } = new List<RevenueReportDetailDisplay>();
    }
    public class RevenueServiceReportPrintVM
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal PriceSubTotal { get; set; }
        public IEnumerable<RevenueReportDetailDisplay> RevenueReportDetailPrints { get; set; } = new List<RevenueReportDetailDisplay>();
    }
    public class RevenueEmployeeAssistantReportDetailDisplay
    {
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceOrigin { get; set; }
        public string PartnerName { get; set; }
        public string EmployeeName { get; set; }
        public string AssistantName { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal PriceSubTotal { get; set; }
        public Guid? GroupById { get; set; }
        public string GroupBy { get; set; }
    }
    public class RevenueEmployeeReportPrintVM
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public decimal PriceSubTotal { get; set; }
        public IEnumerable<RevenueEmployeeAssistantReportDetailDisplay> RevenueReportDetailPrints { get; set; } = new List<RevenueEmployeeAssistantReportDetailDisplay>();
    }

    public class RevenuePartnerReportPrint: RevenuePartnerReportDisplay
    {
        public IEnumerable<RevenueReportDetailDisplay> Lines { get; set; } = new List<RevenueReportDetailDisplay>();
    }
    public class RevenueTimeReportPrint : RevenueTimeReportDisplay
    {
        public IEnumerable<RevenueReportDetailDisplay> Lines { get; set; } = new List<RevenueReportDetailDisplay>();
    }
    public class RevenueServiceReportPrint : RevenueServiceReportDisplay
    {
        public IEnumerable<RevenueReportDetailDisplay> Lines { get; set; } = new List<RevenueReportDetailDisplay>();
    }

    public class RevenueEmployeeReportPrint : RevenueEmployeeReportDisplay
    {
        public IEnumerable<RevenueReportDetailDisplay> Lines { get; set; } = new List<RevenueReportDetailDisplay>();
    }
  
    public class RevenueReportPrintVM<T> where T : class
    {
        public RevenueReportPrintVM(DateTime? dateFrom, DateTime? dateTo)
        {
            this.DateTo = dateTo;
            this.DateFrom = dateFrom;
        }
        public RevenueReportPrintVM() { }
        public CompanyPrintVM Company { get; set;}
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public ApplicationUserSimple User { get; set; }
    }

}
