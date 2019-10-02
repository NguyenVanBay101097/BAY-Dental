using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AccountInvoiceReportByTimeSearch
    {
        /// <summary>
        /// day, month, year
        /// </summary>
        public string GroupBy { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public DateTime? MonthFrom { get; set; }

        public DateTime? MonthTo { get; set; }

        public DateTime? YearFrom { get; set; }

        public DateTime? YearTo { get; set; }
    }

    public class AccountInvoiceReportByTimeItem
    {
        public DateTime? Date { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }

        public string DateStr { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }

    public class AccountInvoiceReportByTimeDetail
    {
        public string Number { get; set; }

        public Guid? InvoiceId { get; set; }

        public DateTime? Date { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }
    }

    public class AccountInvoiceReportByPartnerItem
    {
        public string PartnerName { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }
    }

    public class AccountInvoiceReportByPartnerDetail
    {
        public string Number { get; set; }

        public Guid? InvoiceId { get; set; }

        public string PartnerName { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }
    }

    public class AccountInvoiceReportByProductItem
    {
        public string ProductName { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }
    }

    public class AccountInvoiceReportByProductDetail
    {
        public string ProductName { get; set; }

        public string Number { get; set; }

        public decimal AmountTotal { get; set; }

        public decimal Residual { get; set; }
    }

    public class AccountInvoiceReportHomeSummaryVM
    {
        public int TotalInvoice { get; set; }

        public decimal TotalAmount { get; set; }
    }

    public class AccountInvoiceReportAmountResidual
    {
        public string Name { get; set; }

        public decimal Value { get; set; }
    }

    public class AccountInvoiceReportTopServices
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal ProductQtyTotal { get; set; }
        //public decimal AmountTotal { get; set; }
        //public Guid CompanyId { get; set; }
    }
}
