using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionSettlementReport
    {
        public CommissionSettlementReport()
        {
            Limit = 20;
        }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CompanyId { get; set; }
        public string CommissionType { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }
    }

    public class CommissionSettlementReportRes
    {
        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string CommissionType { get; set; }
        public decimal? Amount { get; set; }

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

        //lưu lại điều kiện lọc
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }
    public class CommissionSettlementDetailReportPar
    {
        public CommissionSettlementDetailReportPar()
        {
            Limit = 20;
        }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CompanyId { get; set; }
        public string CommissionType { get; set; }
        public string Search { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }
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

        /// <summary>
        /// Khách hàng
        /// </summary>
        public string PartnerName { get; set; }

        /// <summary>
        /// Nguồn
        /// </summary>
        public string InvoiceOrigin { get; set; }
        public string EmployeeName { get; set; }
        public string CommissionType { get; set; }
    }
}
