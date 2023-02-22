using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrSalaryConfig: BaseEntity
    {
        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
        /// <summary>
        /// workentrytype cho ngày lễ
        /// </summary>
        public Guid DefaultGlobalLeaveTypeId { get; set; }
        public WorkEntryType DefaultGlobalLeaveType { get; set; }
    }
}
