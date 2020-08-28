using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Phiếu lương
    /// </summary>
    public class HrPayslip: BaseEntity
    {
        public HrPayslip()
        {
            State = "draft";
            TotalAmount = 0;
        }

        /// <summary>
        /// help=Defines the rules that have to be applied to this payslip, accordingly to the contract chosen. If you let empty the field contract, this field isn't mandatory anymore and thus the rules applied will be all the rules set on the structure of all contracts of the employee valid for the chosen period
        /// </summary>
        public Guid? StructId { get; set; }
        public HrPayrollStructure Struct { get; set; }

        /// <summary>
        /// Payslip Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Employee
        /// </summary>
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Date From
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Date To
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Status
        /// ('draft', 'Draft')
        /// ('verify', 'Waiting')
        /// ('done', 'Done')
        /// ('cancel', 'Rejected')
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Company
        /// </summary>
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        /// <summary>
        /// state : 'done'
        /// </summary>
        public Guid? AccountMoveId { get; set; }
        public AccountMove AccountMove { get; set; }

        public ICollection<HrPayslipLine> Lines { get; set; } = new List<HrPayslipLine>();

        public ICollection<HrPayslipWorkedDays> WorkedDaysLines { get; set; } = new List<HrPayslipWorkedDays>();

        public Guid? PayslipRunId { get; set; }
        public HrPayslipRun PayslipRun { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}
