using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Mẫu lương
    /// </summary>
    public class HrPayrollStructure: BaseEntity
    {
        public string Name { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// Kỳ hạn trả lương
        /// </summary>
        public string SchedulePay { get; set; }

        public ICollection<HrSalaryRule> Rules { get; set; } = new List<HrSalaryRule>();
    }
}
