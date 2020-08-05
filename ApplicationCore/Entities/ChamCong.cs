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
        public Employee Employee { get; set; }

        public DateTime? TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        public decimal? HourWorked { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public string Status { get; set; }
    }
}
