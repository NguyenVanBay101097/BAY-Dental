using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionSettlementReport
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class CommissionSettlementReportOutput
    {
        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        /// <summary>
        /// số tiền thanh toán
        /// </summary>
        public decimal? BaseAmount { get; set; }

        /// <summary>
        /// phần trăm hoa hồng
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// tiền hoa hồng bác sĩ
        /// </summary>
        public decimal? Amount { get; set; }
    }

    public class CommissionSettlementReportDetailOutput
    {
        public DateTime? Date { get; set; }
        /// <summary>
        /// Tên dịch vụ
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// số tiền thanh toán
        /// </summary>
        public decimal? BaseAmount { get; set; }

        /// <summary>
        /// phần trăm hoa hồng
        /// </summary>
        public decimal? Percentage { get; set; }

        /// <summary>
        /// tiền hoa hồng bác sĩ
        /// </summary>
        public decimal? Amount { get; set; }
    }
}
