using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeSimple>();
            CreateMap<Employee, EmployeeSimpleContact>();
            CreateMap<Employee, EmployeeBasic>()
                .ForMember(x => x.UserName, x => x.MapFrom(s => s.User.UserName));

            CreateMap<EmployeeDisplay, Employee>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore())
                .ForMember(x => x.Commission, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.StructureType, x => x.Ignore());
            CreateMap<EmployeeSimple, Employee>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore());
            CreateMap<Employee, EmployeeDisplay>()
                .ForMember(x => x.UserName, x => x.MapFrom(x => x.User.UserName))
                .ForMember(x => x.IsUser, x => x.MapFrom(x => x.User != null && x.User.Active))
                .ForMember(x => x.UserCompany, x => x.MapFrom(x => x.User.Company))
                .ForMember(x => x.UserCompanies, x => x.MapFrom(x => x.User.ResCompanyUsersRels.Select(s => s.Company)));

            CreateMap<EmployeeSave, Employee>()
                .ForMember(x => x.Wage, x => x.Ignore())
                .ForMember(x => x.HourlyWage, x => x.Ignore())
                .ForMember(x => x.LeavePerMonth, x => x.Ignore())
                .ForMember(x => x.RegularHour, x => x.Ignore())
                .ForMember(x => x.OvertimeRate, x => x.Ignore())
                .ForMember(x => x.RestDayRate, x => x.Ignore())
                .ForMember(x => x.Allowance, x => x.Ignore());
        }
    }
}
