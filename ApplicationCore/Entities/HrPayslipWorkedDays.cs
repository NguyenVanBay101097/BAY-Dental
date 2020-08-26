using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    public class HrPayslipWorkedDays : BaseEntity
    {
        public string Name { get; set; }

        public Guid PayslipId { get; set; }
        public HrPayslip Payslip { get; set; }

        public int? Sequence { get; set; }

        public string Code { get; set; }

        public decimal? NumberOfDays { get; set; }
        public decimal? Amount { get; set; }
        public decimal? NumberOfHours { get; set; }
    }
}
