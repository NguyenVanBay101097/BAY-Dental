using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Models.ContentEditing
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool Active { get; set; }

        public string Ref { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
        public string IdentityCard { get; set; }

        public string Email { get; set; }

        public DateTime? BirthDay { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? CompanyId { get; set; }

        public bool IsDoctor { get; set; }

        public bool IsAssistant { get; set; }
        public Guid? CommissionId { get; set; }

        public string UserId { get; set; }

        public Guid? StructureTypeId { get; set; }
        public decimal? Wage { get; set; }
        public decimal? HourlyWage { get; set; }
        public DateTime? StartWorkDate { get; set; }
        public string EnrollNumber { get; set; }
        public decimal? LeavePerMonth { get; set; }
        public decimal? RegularHour { get; set; }
        public decimal? OvertimeRate { get; set; }
        public decimal? RestDayRate { get; set; }
        public decimal? Allowance { get; set; }
        public string Avatar { get; set; }
    }
}
