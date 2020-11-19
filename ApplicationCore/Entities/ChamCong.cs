using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// bảng chấm công
    /// </summary>
    public class ChamCong : BaseEntity
    {
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// nhân viên
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// thời gian checkin
        /// </summary>
        public DateTime? TimeIn { get; set; }

        /// <summary>
        /// Thời gian hiển thị trên kanban
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// thời gian checkout
        /// </summary>
        public DateTime? TimeOut { get; set; }

        /// <summary>
        /// số giờ đã làm nếu có đủ checkin và checkout
        /// </summary>
        public decimal? HourWorked { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// trạng thái của chấm công: intitial, process, done
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// loại chấm công: nghỉ phép, workfromhome.....v.v
        /// </summary>
        public Guid? WorkEntryTypeId { get; set; }
        public WorkEntryType WorkEntryType { get; set; }

        ///version 2

        /// work : đi làm - halfaday : nửa ngày - off : ngh
        public string Type { get; set; }

        /// <summary>
        /// tăng ca
        /// </summary>
        public bool OverTime { get; set; }

        /// <summary>
        /// số giờ tăng ca
        /// </summary>
        public decimal? OverTimeHour { get; set; }

    }
}
