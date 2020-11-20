﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class HrPayslipPaged
    {

        public HrPayslipPaged()
        {
            Limit = 20;
        }

        public int Limit { get; set; }
        public int Offset { get; set; }
        public string Search { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string State { get; set; }
        public Guid? PayslipRunId { get; set; }
        public Guid? EmployeeId { get; set; }

    }

    public class HrPayslipSave
    {
        public Guid Id { get; set; }
        public Guid? StructId { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public Guid EmployeeId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
        public string State { get; set; }

        public IEnumerable<HrPayslipWorkedDaySave> ListHrPayslipWorkedDaySave { get; set; } = new List<HrPayslipWorkedDaySave>();

        public IEnumerable<HrPayslipWorkedDaySave> WorkedDaysLines { get; set; } = new List<HrPayslipWorkedDaySave>();
        public IEnumerable<HrPayslipLineSave> Lines { get; set; } = new List<HrPayslipLineSave>();

        public Guid CompanyId { get; set; }

        public Guid? StructureTypeId { get; set; }
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
        public decimal? AmercementMoney { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? ActualLeavePerMonth { get; set; }
        public decimal? LeavePerMonthUnpaid { get; set; }
    }

    public class HrPayslipDisplay
    {
        public Guid Id { get; set; }

        public Guid? StructId { get; set; }
        public HrPayrollStructureBasic Struct { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public Guid EmployeeId { get; set; }
        public EmployeeBasic Employee { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string State { get; set; }

        public Guid CompanyId { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid? StructureTypeId { get; set; }
        public HrPayrollStructureTypeSimple StructureType { get; set; }

        public IEnumerable<HrPayslipWorkedDayDisplay> WorkedDaysLines { get; set; } = new List<HrPayslipWorkedDayDisplay>();
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
        public decimal? AmercementMoney { get; set; }
        public decimal? NetSalary { get; set; }
        public decimal? ActualLeavePerMonth { get; set; }
        public decimal? LeavePerMonthUnpaid { get; set; }

    }

    public class HrPayslipOnChangeEmployee
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public Guid? EmployeeId { get; set; }
    }

    public class HrPayslipDefaultGet
    {
        //Tạm thời để trống, muốn mở rộng sau cũng dễ
    }

    public class HrPayslipBasic
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public string EmployeeName { get; set; }

        public string State { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string PayslipRunName { get; set; }

        public Guid? PayslipRunId { get; set; }

        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
