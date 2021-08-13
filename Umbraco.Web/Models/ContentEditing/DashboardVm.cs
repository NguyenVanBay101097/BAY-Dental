﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{

    public class GetDefaultRequest
    {
        public Guid? AppointmentId { get; set; }
    }


    public class GetCountMedicalXamination
    {
        public int NewMedical { get; set; }
        public int ReMedical { get; set; }
    }

    public class CustomerReceiptRequest
    {
        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaiting { get; set; }

        /// <summary>
        /// Thời gian dự kiến
        /// </summary>
        public int TimeExpected { get; set; }

        /// <summary>
        /// Danh sách dịch vụ
        /// </summary>
        public IEnumerable<ProductSimple> Products { get; set; } = new List<ProductSimple>();

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Khách hàng tái khám
        /// </summary>
        public bool IsRepeatCustomer { get; set; }

        public Guid? AppointmentId { get; set; }
    }

    public class CustomerReceiptReqonse
    {
        public CustomerReceiptBasic CustomerReceipt { get; set; }
        public AppointmentBasic Appointment { get; set; }
    }

    public class ReportTodayRequest
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public Guid? CompanyId { get; set; }
    }

    public class RevenueTodayReponse
    {
        public RevenueTodayReponse()
        {
            TotalAmountYesterday = 0;
            TotalOther = 0;
            TotalBank = 0;
            TotalCash = 0;
            TotalAmount = 0;
        }
      
        public decimal TotalBank { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalOther{ get; set; }
        public decimal TotalAmountYesterday { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class SumaryRevenueReport
    {
        public string Type { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Balance { get; set; }
    }

    public class SumaryRevenueReportFilter
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
        /// debt: công nợ
        /// advance: tạm ứng
        /// cash_bank: TM/NH
        /// </summary>
        public string ResultSelection { get; set; }

        public string AccountCode { get; set; }
    }

}
