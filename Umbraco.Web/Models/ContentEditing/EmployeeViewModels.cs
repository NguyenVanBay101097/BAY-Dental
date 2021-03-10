using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EmployeeSimple
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Guid? PartnerId { get; set; }
    }

    public class EmployeeSimpleContact
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
    }


    public class EmployeeBasic
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Ref { get; set; }

        public string Phone { get; set; }


        public string Email { get; set; }
        public Guid? CategoryId { get; set; }
        public EmployeeCategoryBasic Category { get; set; }
        public decimal? Wage { get; set; }
        public Guid? StructureTypeId { get; set; }
        /// <summary>
        /// Số ngày nghỉ/tháng
        /// </summary>
        public decimal? LeavePerMonth { get; set; }

        /// <summary>
        /// số giờ công chuẩn 1 ngày
        /// </summary>
        public decimal? RegularHour { get; set; }

        /// <summary>
        /// tỉ lệ lương tăng ca
        /// </summary>
        public decimal? OvertimeRate { get; set; }
        /// <summary>
        /// tỉ lệ lương ngày đi làm thêm
        /// </summary>
        public decimal? RestDayRate { get; set; }
        /// <summary>
        /// trợ cấp
        /// </summary>
        public decimal? Allowance { get; set; }

        public bool Active { get; set; }

        public string UserName { get; set; }

        public Guid CompanyId { get; set; }
        public CompanyBasic Company { get; set; }
    }

    public class EmployeeDisplay
    {
        public EmployeeDisplay()
        {
            IsDoctor = false;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// CMND
        /// </summary>
        public string IdentityCard { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// Là bác sĩ
        /// </summary>
        public bool? IsDoctor { get; set; }

        /// <summary>
        /// kết nối hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }
        public CommissionBasic Commission { get; set; }

        public decimal? Wage { get; set; }
        public decimal? HourlyWage { get; set; }
        public DateTime? StartWorkDate { get; set; }

        public string EnrollNumber { get; set; }
        public decimal? LeavePerMonth { get; set; }
        public decimal? RegularHour { get; set; }
        public decimal? OvertimeRate { get; set; }
        public decimal? RestDayRate { get; set; }
        public decimal? Allowance { get; set; }

        public bool IsUser { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public CompanyBasic UserCompany { get; set; }

        public IEnumerable<CompanyBasic> UserCompanies { get; set; } = new List<CompanyBasic>();

        public Guid? UserId { get; set; }
        public string Avatar { set; get; }
        public string UserAvatar { get; set; }
        public bool IsAllowSurvey { get; set; }
        public Guid? GroupId { get; set; }
        public ResGroupBasic Group { get; set; }
    }

    public class EmployeePaged
    {
        public EmployeePaged()
        {
            Limit = 20;
        }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public string Search { get; set; }

        //group filter IsDoctor và IsAssistant
        public bool? IsDoctor { get; set; }
        public bool? IsAssistant { get; set; }
        //public bool? IsOther { get; set; }
        public string Position { get; set; }

        public IEnumerable<Guid> Ids { get; set; }

        public bool? Active { get; set; }
        public bool? IsAllowSurvey { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class EmployeeSave
    {
        public string Name { get; set; }

        /// <summary>
        /// Mã
        /// </summary>
        public string Ref { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// CMND
        /// </summary>
        public string IdentityCard { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        /// <summary>
        /// Là bác sĩ
        /// </summary>
        public bool IsDoctor { get; set; }

        /// <summary>
        /// hoa hồng
        /// </summary>
        public Guid? CommissionId { get; set; }

        public decimal? Wage { get; set; }

        public decimal? HourlyWage { get; set; }

        public DateTime? StartWorkDate { get; set; }

        public string EnrollNumber { get; set; }

        public decimal? LeavePerMonth { get; set; }

        public decimal? RegularHour { get; set; }

        public decimal? OvertimeRate { get; set; }

        public decimal? RestDayRate { get; set; }

        public decimal? Allowance { get; set; }

        public bool IsUser { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public Guid? UserCompanyId { get; set; }

        public IEnumerable<Guid> UserCompanyIds { get; set; } = new List<Guid>();

        public bool CreateChangePassword { get; set; }

        public string Avatar { get; set; }
        public string UserAvatar { get; set; }
        public bool IsAllowSurvey { get; set; }
        public Guid? GroupId { get; set; }
    }

    public class EmployeeActive
    {
        public bool Active { get; set; }
    }


    public class EmployeeSurveyDisplay
    {
        public Guid Id { get; set; }
        public string Ref { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public int TotalAssignment { get; set; }
        public int DoneAssignment { get; set; }
        public int FollowAssignment { get { return this.TotalAssignment - this.DoneAssignment; } set { } }
    }
}
