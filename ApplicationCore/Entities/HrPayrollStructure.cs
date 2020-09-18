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
        public HrPayrollStructure()
        {
            Active = true;
            UseWorkedDayLines = true;
            SchedulePay = "monthly";
        }

        public string Name { get; set; }

        public bool Active { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// Check this option if this structure is the common one
        /// </summary>
        public bool RegularPay { get; set; }

        public Guid TypeId { get; set; }
        public HrPayrollStructureType Type { get; set; }

        /// <summary>
        /// Worked days won't be computed/displayed in payslips.
        /// </summary>
        public bool UseWorkedDayLines { get; set; }

        /// <summary>
        /// Kỳ hạn trả lương
        /// </summary>
        public string SchedulePay { get; set; }

        public ICollection<HrSalaryRule> Rules { get; set; } = new List<HrSalaryRule>();

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
