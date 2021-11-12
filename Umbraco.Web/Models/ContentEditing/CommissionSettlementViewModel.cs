﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CommissionSettlementFilterReport
    {
        public CommissionSettlementFilterReport()
        {
            Limit = 20;
        }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? AgentId { get; set; }

        public Guid? CompanyId { get; set; }
        public string CommissionType { get; set; }

        public string Search { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }

        /// <summary>
        /// employee : nhân viên
        /// agent : người giới thiệu
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// customer : khách hàng
        /// employee : nhân viên
        /// partner : đối tác
        /// </summary>
        public string Classify { get; set; }

        /// <summary>
        /// "" : hiển thị tất cả
        /// "greater_than_zero": hiển thị hoa hồng > 0
        /// "equals_zero": hiển thị hoa hồng = 0
        /// </summary>
        public string CommissionDisplay { get; set; }
    }

    public class CommissionSettlementReportRes
    {
        public string EmployeeName { get; set; }
        public Guid? EmployeeId { get; set; }
        public string CommissionType { get; set; }
        public decimal? Amount { get; set; }

    }

    public class CommissionSettlementReportExcelRes
    {
        public string EmployeeName { get; set; }
        [EpplusIgnore]
        public string CommissionType { get; set; }
        public string CommissionTypeDisplay
        {
            get
            {
                switch (this.CommissionType)
                {
                    case "doctor":
                        return "Bác sĩ";
                    case "assistant":
                        return "Phụ tá";
                    case "counselor":
                        return "Tư vấn";
                    default:
                        return "";
                };
            }
            set { }
        }

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
        public Guid? SaleOrderId { get; set; }
        public string Name { get; set; }
        public string CommissionType { get; set; }

        /// <summary>
        /// chỉ sử dụng cho người giới thiệu
        /// customer : khách hàng
        /// employee : nhân viên
        /// partner : đối tác
        /// </summary>
        public string Classify { get; set; }

        public decimal? TotalAmount { get; set; }
    }

    public class CommissionSettlementReportDetailOutputExcel
    {
        public DateTime? Date { get; set; }
        public string EmployeeName { get; set; }
        [EpplusIgnore]
        public string CommissionType { get; set; }
        public string CommissionTypeDisplay
        {
            get
            {
                switch (this.CommissionType)
                {
                    case "doctor":
                        return "Bác sĩ";
                    case "assistant":
                        return "Phụ tá";
                    case "counselor":
                        return "Tư vấn";
                    default:
                        return "";
                };
            }
            set { }
        }
        public string InvoiceOrigin { get; set; }
        public string PartnerName { get; set; }
        public string ProductName { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? Amount { get; set; }
    }

    public class CommissionSettlementReportExportExcelPar
    {

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CompanyId { get; set; }
        public string CommissionType { get; set; }

    }

    public class CommissionSettlementDetailReportExcelPar
    {

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public Guid? CompanyId { get; set; }
        public string CommissionType { get; set; }
        public string Search { get; set; }
    }

    public class CommissionSettlementOverviewFilter
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Classify { get; set; }

        public string GroupBy { get; set; }
    }

    public class CommissionSettlementOverview
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Classify { get; set; }

        /// <summary>
        /// tổng tiền lợi nhuận
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// Tổng tiền hoa hồng
        /// </summary>
        public decimal Amount { get; set; }
    }

    public class SumAmountTotalReponse {

        /// <summary>
        /// Tổng tiên thanh toán
        /// </summary>
        public decimal TotalAmount { get; set; }

        //Tong tiền lợi nhuận
        public decimal TotalBaseAmount { get; set; }

        //Tong tiền hoa hồng
        public decimal TotalComissionAmount { get; set; }
    }
}
