using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class ResourceCalendar: BaseEntity
    {
        public string Name { get; set; }

        /// <summary>
        /// Lấy mặc định company id của user đang đăng nhập
        /// </summary>
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<ResourceCalendarAttendance> Attendances { get; set; } = new List<ResourceCalendarAttendance>();
        /// <summary>
        /// Số giờ trung bình trên 1 ngày
        /// </summary>
        public decimal? HoursPerDay { get; set; }
    }
}
