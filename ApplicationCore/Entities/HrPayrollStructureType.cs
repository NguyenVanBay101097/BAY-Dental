using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrPayrollStructureType: BaseEntity
    {
        /// <summary>
        /// Thời gian làm việc mặc định
        /// </summary>
        public Guid? DefaultResourceCalendarId { get; set; }
        public ResourceCalendar DefaultResourceCalendar { get; set; }

        /// <summary>
        /// Định kỳ trả lương mặc định
        /// monthly
        /// quarterly
        /// semi-annually
        /// annually
        /// weekly
        /// bi-weekly
        /// bi-monthly
        /// </summary>
        public string DefaultSchedulePay { get; set; }

        public Guid? DefaultStructId { get; set; }
        public HrPayrollStructure DefaultStruct { get; set; }

        /// <summary>
        /// help=Work entry type for regular attendances.
        /// </summary>
        public Guid? DefaultWorkEntryTypeId { get; set; }
        public WorkEntryType DefaultWorkEntryType { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Loại lương bổng
        /// monthly: Lương tháng cố định
        /// hourly: Lương theo giờ
        /// </summary>
        public string WageType { get; set; }
    }
}
