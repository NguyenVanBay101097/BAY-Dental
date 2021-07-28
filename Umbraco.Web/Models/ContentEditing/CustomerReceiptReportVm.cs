using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CustomerReceiptReportFilter
    {
        public CustomerReceiptReportFilter()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public int? TimeFrom { get; set; }
        public int? TimeTo { get; set; }

        public string Search { get; set; }
        public Guid? CompanyId { get; set; }

        public Guid? DoctorId { get; set; }

        /// <summary>
        /// Khách hàng tái khám
        /// </summary>
        public bool? IsRepeatCustomer { get; set; }

        /// <summary>
        /// không điều trị
        /// </summary>
        public bool? IsNoTreatment { get; set; }

        public string state { get; set; }

    }

    public class CustomerReceiptTimeDetailFilter
    {
        public CustomerReceiptTimeDetailFilter()
        {
            Limit = 10;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public int Time { get; set; }
        public Guid? CompanyId { get; set; }
    }

    public class CustomerReceiptReportBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaiting { get; set; }

        /// <summary>
        /// thời gian đang khám
        /// </summary>
        public DateTime? DateExamination { get; set; }

        /// <summary>
        /// thời gian hoàn thành
        /// </summary>
        public DateTime? DateDone { get; set; }

        /// <summary>
        /// Thời gian dự kiến
        /// </summary>
        public int TimeExpected { get; set; }

        /// <summary>
        /// Danh sách dịch vụ
        /// </summary>
        public string Products { get; set; }

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public PartnerSimple Partner { get; set; }

        public string DoctorName { get; set; }


        /// <summary>
        /// waiting : chờ khám
        /// examination : đang khám 
        /// done : hoàn thành
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// lý do không điều trị
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Khách hàng tái khám
        /// </summary>
        public bool IsRepeatCustomer { get; set; }


        /// <summary>
        /// không điều trị
        /// </summary>
        public bool IsNoTreatment { get; set; }

        public int? MinuteWaiting { get; set; }
        public int? MinuteExamination { get; set; }
        public int? MinuteTotal { get; set; }
    }

    public class CustomerReceiptReportTime
    {
        public string TimeRange { get; set; }
        public int Time { get; set; }
        public int TimeRangeCount { get; set; }
    }

    public class CustomerReceiptGetCountItem
    {
        public string Name { get; set; }
        public int CountCustomerReceipt { get; set; }
        public int TotalCustomerReceipt { get; set; }
    }

}
