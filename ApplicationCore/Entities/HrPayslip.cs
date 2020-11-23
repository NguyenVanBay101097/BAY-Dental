using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
    /// <summary>
    /// Phiếu lương
    /// </summary>
    public class HrPayslip : BaseEntity
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

        public Guid? StructureTypeId { get; set; }
        public HrPayrollStructureType StructureType { get; set; }
        /// <summary>
        /// lương 1 ngày
        /// </summary>
        public decimal? DaySalary { get; set; }
        /// <summary>
        /// số ngày công thực tế
        /// </summary>
        public decimal? WorkedDay { get; set; }
        /// <summary>
        /// tổng lương cơ bản dựa trên lương 1 ngày
        /// </summary>
        public decimal? TotalBasicSalary { get; set; }
        /// <summary>
        /// số giờ tăng ca
        /// </summary>
        public decimal? OverTimeHour { get; set; }
        /// <summary>
        /// tổng tiền tăng ca
        /// </summary>
        public decimal? OverTimeHourSalary { get; set; }
        /// <summary>
        /// số ngày làm thêm vào ngày nghỉ 
        /// </summary>
        public decimal? OverTimeDay { get; set; }
        /// <summary>
        /// tính dựa trên RESTDAYRATE trong bảng nhân viên
        /// </summary>
        public decimal? OverTimeDaySalary { get; set; }
        /// <summary>
        /// phụ cấp xác định lấy từ bảng nhân viên
        /// </summary>
        public decimal? Allowance { get; set; }
        /// <summary>
        /// phụ cấp khác
        /// </summary>
        public decimal? OtherAllowance { get; set; }
        /// <summary>
        /// lương thưởng
        /// </summary>
        public decimal? RewardSalary { get; set; }
        /// <summary>
        /// phụ cấp ngày lễ tết
        /// </summary>
        public decimal? HolidayAllowance { get; set; }
        /// <summary>
        /// tổng cộng lương:tổng các khoản lương chưa có hoa hồng
        /// </summary>
        public decimal? TotalSalary { get; set; }
        /// <summary>
        /// lương hoa hồng
        /// </summary>
        public decimal? CommissionSalary { get; set; }
        /// <summary>
        /// tạm ứng lương
        /// </summary>
        public decimal? AdvancePayment { get; set; }

        /// <summary>
        /// tiền bị phạt
        /// </summary>
        public decimal? AmercementMoney { get; set; }
        /// <summary>
        /// tiền thực nhận: bằng totalsalary + commission - amercement - advance
        /// </summary>
        public decimal? NetSalary { get; set; }
        /// <summary>
        /// số ngày nghỉ thực tế của tháng đó
        /// </summary>
        public decimal? ActualLeavePerMonth { get; set; }
        /// <summary>
        /// số ngày nghỉ không lương
        /// </summary>
        public decimal? LeavePerMonthUnpaid { get; set; }

        /// <summary>
        /// phiếu chi lương
        /// </summary>
        public Guid? SalaryPaymentId { get; set; }
        public SalaryPayment SalaryPayment { get; set; }



    }
}
