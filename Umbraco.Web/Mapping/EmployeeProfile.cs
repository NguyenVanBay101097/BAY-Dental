using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
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
            CreateMap<Employee, EmployeeBasic>();

            CreateMap<EmployeeDisplay, Employee>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore())
                .ForMember(x => x.Commission, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore());

            CreateMap<EmployeeSimple, Employee>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore());
            CreateMap<Employee, EmployeeDisplay>();
        }
    }
}
