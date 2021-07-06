using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class CustomerReceiptPaged
    {
        public CustomerReceiptPaged()
        {
            Limit = 20;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string Search { get; set; }

        public Guid? DoctorId { get; set; }

        public string State { get; set; }

    }

    public class CustomerReceiptBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaitting { get; set; }

        /// <summary>
        /// thời gian đang khám
        /// </summary>
        public DateTime? DateExamination { get; set; }

        /// <summary>
        /// thời gian hoàn thành
        /// </summary>
        public DateTime? DateDone { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public string DoctorName { get; set; }

        /// <summary>
        /// khach hang
        /// </summary>
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; }

        public string State { get; set; }

    }

    public class CustomerReceiptDisplay
    {
        public Guid Id { get; set; }

        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaitting { get; set; }

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
        public IEnumerable<ProductSimple> Products { get; set; } = new List<ProductSimple>();

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public PartnerSimpleContact Partner { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }


        /// <summary>
        /// waitting : chờ khám
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
    }

    public class CustomerReceiptSave
    {
        /// <summary>
        /// thời gian chờ khám
        /// </summary>
        public DateTime? DateWaitting { get; set; }

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
        /// waitting : chờ khám
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
    }

    public class CustomerReceiptStatePatch
    {
        public string State { get; set; }

        public bool IsNoTreatment { get; set; }

        public string Reason { get; set; }
    }

    public class CustomerReceiptGetCountVM
    {
        public string Search { get; set; }
        public Guid? DoctorId { get; set; }
        public string State { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

    }
}
