using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// view from CustomerReceipt : customer_receipt_report
    /// </summary>
    public class CustomerReceiptReport
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

        /// <summary>
        /// Người tiếp nhận khách hàng
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? DoctorId { get; set; }
        public Employee Doctor { get; set; }


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
}
