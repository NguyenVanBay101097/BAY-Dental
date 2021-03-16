using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Quản lý cuộc hẹn
    /// </summary>
    public class Appointment : BaseEntity
    {
        /// <summary>
        /// Mã phiếu hẹn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ngày hẹn
        /// </summary>
        public DateTime Date { get; set; }

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
        public ICollection<ProductAppointmentRel> AppointmentServices { get; set; } = new List<ProductAppointmentRel>();

        /// <summary>
        /// Ghi chú, nội dung
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Người hẹn
        /// </summary>
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        //Hẹn khách hàng nào?
        public Guid PartnerId { get; set; }
        public Partner Partner { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public Guid? DotKhamId { get; set; }
        public DotKham DotKham { get; set; }

        public Guid? DoctorId { get; set; }
        public Employee Doctor { get; set; }


        /// <summary>
        /// Trạng thái cuộc hẹn: Đang hẹn, Chờ khám, Đang khám, Hoàn thành, Hủy hẹn
        /// confirmed, waiting, examination, done, cancel
        /// </summary>
        public string State { get; set; }

        public string Reason { get; set; }

        public ICollection<AppointmentMailMessageRel> AppointmentMailMessageRels { get; set; } = new List<AppointmentMailMessageRel>();

        public Guid? SaleOrderId { get; set; }
        public SaleOrder SaleOrder { get; set; }
    }
}
