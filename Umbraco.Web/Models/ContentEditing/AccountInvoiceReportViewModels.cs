using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class RevenueReportQueryCommon
    {
        public RevenueReportQueryCommon(DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            this.DateFrom = dateFrom;
            this.DateFrom = dateFrom;
            this.CompanyId = companyId;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? CompanyId { get; set; }
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
        public bool EmployeeGroup { get; set; }
        public Guid? EmployeeId { get; set; }

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
    }

    public class RevenueServiceReportDisplay
    {
        public string ProductName { get; set; }
        public Guid? ProductId { get; set; }
        public decimal PriceSubTotal { get; set; }
    }

    public class RevenueEmployeeReportDisplay
    {
        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        public decimal PriceSubTotal { get; set; }
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
        public DateTime? Date { get; set; }
        public Guid? ProductId { get; set; }
        public bool EmployeeGroup { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool AssistantGroup { get; set; }
        public Guid? AssistantId { get; set; }
    }

    public class RevenueReportDetailDisplay
    {
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceOrigin { get; set; }
        public string PartnerName { get; set; }
        public string EmployeeName { get; set; }
        public string AssistantName { get; set; }
        public string ProductName { get; set; }
        public decimal PriceSubTotal { get; set; }
    }

}
