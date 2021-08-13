using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CashBookSearch
    {
        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// bank: ngan hang
        /// cash: tien mat
        /// cash_bank: tong quy
        /// </summary>
        public string ResultSelection { get; set; }
    }

    public class CashBookDetailFilter
    {
        public CashBookDetailFilter()
        {
            Limit = 20;
        }

        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// bank: ngan hang
        /// cash: tien mat
        /// cash_bank: tong quy
        /// </summary>
        public string ResultSelection { get; set; }

        public string Search { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }

    public class CashBookReportFilter
    {
        public DateTime? DateTo { get; set; }

        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// groupby:day group theo ngay
        /// groupby:month group theo tháng
        /// </summary>
        public string GroupBy { get; set; }
    }

    public class SumaryCashBookFilter
    {
        /// <summary>
        /// Ngay bat dau
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// ngay ket thuc
        /// </summary>
        public DateTime? DateFrom { get; set; }

        public Guid? CompanyId { get; set; }

        /// <summary>
        /// debt: ghi công nợ
        /// advance: tạm ứng
        /// cash_bank: tong quy
        /// </summary>
        public string ResultSelection { get; set; }

        /// <summary>
        /// customer
        /// supplier
        /// </summary>
        public string PartnerType { get; set; }

        public string AccountCode { get; set; }
    }

    public class SumaryCashBook
    {
        public string Type { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Balance { get; set; }
    }
}
