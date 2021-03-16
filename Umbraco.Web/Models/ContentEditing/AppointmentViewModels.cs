using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class AppointmentBasic
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã phiếu hẹn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ngày hẹn
        /// </summary>
        public DateTime? Date { get; set; }

        public string Time { get; set; }

        /// <summary>
        /// Người hẹn
        /// </summary>
        public string UserId { get; set; }
        //public ApplicationUserSimple User { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public string DoctorName { get; set; }

        /// <summary>
        /// Trạng thái cuộc hẹn: xác nhận, khách đã tới hoặc đã hủy bỏ
        /// confirmed, done, cancel
        /// </summary>
        public string State { get; set; }
        public string Reason { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        //public PartnerSimpleContact Partner { get; set; }
        public string PartnerName { get; set; }
        public string PartnerPhone { get; set; }

        public string PartnerDisplayName { get; set; }

        public PartnerBasic Partner { get; set; }

        public string Note { get; set; }
    }

    public class AppointmentDisplay
    {
        public AppointmentDisplay()
        {
            Date = DateTime.Now;
            State = "confirmed";
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Mã phiếu hẹn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ngày hẹn
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// thời gian hẹn
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Thời gian dự kiến
        /// </summary>
        public string TimeExpected { get; set; }

        /// <summary>
        /// Danh sách dịch vụ
        /// </summary>
        public IEnumerable<ProductSimple> Services { get; set; } = new List<ProductSimple>();

        /// <summary>
        /// Trạng thái cuộc hẹn: xác nhận, khách đã tới hoặc đã hủy bỏ
        /// confirmed, done, cancel
        /// </summary>
        public string State { get; set; }
        public string Reason { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public PartnerSimpleInfo Partner { get; set; }

        public Guid CompanyId { get; set; }

        public Guid? DotKhamId { get; set; }

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Bác sĩ
        /// </summary>
        public Guid? DoctorId { get; set; }
        public EmployeeSimple Doctor { get; set; }

        public bool HasDotKhamRef { get; set; }
        public Guid? SaleOrderId { get; set; }
    }
    
    public class AppointmentPaged
    {
        public AppointmentPaged()
        {
            Offset = 0;
        }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public Guid? PartnerId { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeTo { get; set; }

        public string State { get; set; }

        public Guid? DotKhamId { get; set; }

        public string UserId { get; set; }
        public Guid? SaleOrderId { get; set; }

        public Guid? CompanyId { get; set; }
        public Guid? DoctorId { get; set; }
    }

    public class AppointmentSearch
    {
        public string Search { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeTo { get; set; }

        public string State { get; set; }

        public Guid? DotKhamId { get; set; }
    }

    public class AppointmentSearchByDate
    {
        public string State { get; set; }

        public DateTime? Date { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Search { get; set; }

        public string UserId { get; set; }

        public Guid? EmployeeId { get; set; }
    }

    public class AppointmentDefaultGet
    {
        public Guid? DotKhamId { get; set; }

        public Guid? PartnerId { get; set; }
        public Guid? SaleOrderId { get; set; }
    }

    public class AppointmentStateCount
    {
        public string State { get; set; }
        public string Color { get; set; }
        public int Count { get; set; }
    }

    public class DateFromTo
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class AppointmentPatch
    {
        public Guid Id { get; set; }
        public string State { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }
    }

    public class AppointmentStatePatch
    {
        public string State { get; set; }
        public string Reason { get; set; }
    }

    public class AppointmentGetCountVM
    {
        public string State { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }

}
