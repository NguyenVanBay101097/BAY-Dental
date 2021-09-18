using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Models.PrintTemplate
{
    public class HrPayslipRunPrintTemplate
    {
        public string Name { get; set; }

        public CompanyPrintTemplate Company { get; set; }

        public string State { get; set; }
        public DateTime? DateSalary { get; set; }

        public string CreatedById { get; set; }
        public IEnumerable<HrPayslipPrintTemplate> Slips { get; set; } = new List<HrPayslipPrintTemplate>();
        public string UserName { get; set; }
        public bool IsExistSalaryPayment { get; set; }
    }

    public class HrPayslipPrintTemplate
    {
        public HrPayrollStructureBasicPrintTemplate Struct { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public EmployeePrintTemplate Employee { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string State { get; set; }

        public Guid CompanyId { get; set; }

        public decimal TotalAmount { get; set; }

        public HrPayrollStructureTypePrintTemplate StructureType { get; set; }

        public IEnumerable<HrPayslipWorkedDayPrintTemplate> WorkedDaysLines { get; set; } = new List<HrPayslipWorkedDayPrintTemplate>();
        public Guid? PayslipRunId { get; set; }
        public decimal? DaySalary { get; set; }
        public decimal? WorkedDay { get; set; }
        public decimal? TotalBasicSalary { get; set; }
        public decimal? OverTimeHour { get; set; }
        public decimal? OverTimeHourSalary { get; set; }
        public decimal? OverTimeDay { get; set; }
        public decimal? OverTimeDaySalary { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? OtherAllowance { get; set; }
        public decimal? RewardSalary { get; set; }
        public decimal? HolidayAllowance { get; set; }
        public decimal? TotalSalary { get; set; }
        public decimal? CommissionSalary { get; set; }
        public decimal? Tax { get; set; }
        public decimal? SocialInsurance { get; set; }
        public decimal? AmercementMoney { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? ActualLeavePerMonth { get; set; }
        public decimal? LeavePerMonthUnpaid { get; set; }
        public SalaryPaymentBasicPrintTemplate SalaryPayment { get; set; }
    }

    public class HrPayrollStructureBasicPrintTemplate
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public bool RegularPay { get; set; }
    }

    public class HrPayrollStructureTypePrintTemplate
    {
        public string Name { get; set; }
        public string WageType { get; set; }
    }

    public class HrPayslipWorkedDayPrintTemplate
    {
        public string Name { get; set; }

        public decimal? NumberOfDays { get; set; }

        public decimal? NumberOfHours { get; set; }

        public decimal? Amount { get; set; }
    }

    public class SalaryPaymentBasicPrintTemplate
    {
        /// <summary>
        /// Ngày lập phiếu
        /// </summary>
        public DateTime Date { get; set; }

        public string JournalName { get; set; }

        public string State { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Số tiền
        /// </summary>
        public decimal Amount { get; set; }

        public string EmployeeName { get; set; }
    }
}
