using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrSalaryRule: BaseEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public Guid CategoryId { get; set; }
        public HrSalaryRuleCategory Category { get; set; }

        public int? Sequence { get; set; }

        public bool Active { get; set; }

        public Guid? CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// The computation method for the rule amount.
        /// </summary>
        public string AmountSelect { get; set; }

        /// <summary>
        /// Fixed Amount
        /// </summary>
        public decimal? AmountFix { get; set; }

        /// <summary>
        /// Percentage (%)
        /// </summary>
        public decimal? AmountPercentage { get; set; }

        public bool AppearsOnPayslip { get; set; }

        public string Note { get; set; }

        public Guid StructId { get; set; }
        public HrPayrollStructure Struct { get; set; }
    }
}
