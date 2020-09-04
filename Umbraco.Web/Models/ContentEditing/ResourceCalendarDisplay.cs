using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Models
{
    public class ResourceCalendarDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Lấy mặc định company id của user đang đăng nhập
        /// </summary>
        public Guid? CompanyId { get; set; }
        public IEnumerable<ResourceCalendarAttendanceDisplay> Attendances { get; set; } = new List<ResourceCalendarAttendanceDisplay>();
        /// <summary>
        /// Số giờ trung bình trên 1 ngày
        /// </summary>
        public decimal? HoursPerDay { get; set; }
    }
}
