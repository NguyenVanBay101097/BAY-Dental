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

        public string ResultSelection { get; set; }
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
}
