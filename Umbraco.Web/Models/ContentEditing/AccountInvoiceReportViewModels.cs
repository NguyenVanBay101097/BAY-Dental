using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class AccountInvoiceReportPaged
    {
        public AccountInvoiceReportPaged()
        {
            Limit = 20;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Search { get; set; }
        public Guid? CompanyId { get; set; }
        /// <summary>
        /// InvoiceDate, ProductId, EmployeeId, AssistantId
        /// </summary>
        public string GroupBy { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
    }

    public class AccountInvoiceReportDetailPaged
    {
        public AccountInvoiceReportDetailPaged()
        {
            Limit = 20;
        }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Search { get; set; }
        public Guid? CompanyId { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public DateTime? Date { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? AssistantId { get; set; }
        /// <summary>
        /// InvoiceDate, ProductId, EmployeeId, AssistantId
        /// </summary>
        public string GroupBy { get; set; }
    }

    public class AccountInvoiceReportDisplay
    {
        public DateTime? InvoiceDate { get; set; }
        public string ProductName { get; set; }
        public Guid? ProductId { get; set; }
        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string AssistantName { get; set; }
        public Guid? AssistantId { get; set; }
        public decimal PriceSubTotal { get; set; }
    }

    public class AccountInvoiceReportDetailDisplay
    {
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceOrigin { get; set; }
        public string PartnerName { get; set; }
        public string EmployeeName { get; set; }
        public string AssistantName { get; set; }
        public string ProductName { get; set; }
        public decimal PriceSubTotal { get; set; }
    }

    public class AccountInvoiceReportExcel : AccountInvoiceReportDisplay
    {
        public IEnumerable<AccountInvoiceReportDetailDisplay> Lines { get; set; } = new List<AccountInvoiceReportDetailDisplay>();
    }


}
