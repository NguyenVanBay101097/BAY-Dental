using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrPayslipLine : BaseEntity
    {
        public HrPayslipLine()
        {
            Amount = 0;
        }
        public string Name { get; set; }

        public string Code { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Total { get; set; }

        public Guid SlipId { get; set; }
        public HrPayslip Slip { get; set; }

        public Guid SalaryRuleId { get; set; }
        public HrSalaryRule SalaryRule { get; set; }

        public decimal? Rate { get; set; }

        public Guid? CategoryId { get; set; }
        public HrSalaryRuleCategory Category { get; set; }

        public int? Sequence { get; set; }


    }
}
