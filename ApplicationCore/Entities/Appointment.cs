using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Quản lý cuộc hẹn
    /// </summary>
    public class Appointment: BaseEntity
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
        /// Trạng thái cuộc hẹn: xác nhận, khách đã tới hoặc đã hủy bỏ
        /// confirmed, done, cancel
        /// </summary>
        public string State { get; set; }

        public ICollection<AppointmentMailMessageRel> AppointmentMailMessageRels { get; set; } = new List<AppointmentMailMessageRel>();
    }
}
