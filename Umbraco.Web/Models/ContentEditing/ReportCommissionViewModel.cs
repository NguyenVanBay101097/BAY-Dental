using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    
    public class CommissionReport
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public decimal? EstimateTotal { get; set; }
        public decimal? CommissionTotal { get; set; }
    }

    public class CommissionReportItem
    {
        public string UserId { get; set; }
        public string Name { get; set; }

        public DateTime? Date { get; set; }
        /// <summary>
        /// dịch vụ
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Tổng tiền
        /// </summary>
        public decimal? AmountTotal { get; set; }

        /// <summary>
        /// Đã thanh toán
        /// </summary>
        public decimal? PrepaidTotal { get; set; }

        /// <summary>
        /// phần trăm hoa hồng
        /// </summary>
        public decimal? PercentCommission { get; set; }

        /// <summary>
        /// hoa hồng dự tính
        /// </summary>
        public decimal? EstimateTotal { get; set; }

        /// <summary>
        /// tiền hoa hồng thực tế
        /// </summary>
        public decimal? CommissionTotal { get; set; }
    }

    public class ReportFilterCommission
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string UserId { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class ReportFilterCommissionDetail
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string UserId { get; set; }
    }
}
