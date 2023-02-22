using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class EmployeeCategoryProfile : Profile
    {
        public EmployeeCategoryProfile()
        {
            CreateMap<EmployeeCategory, EmployeeCategoryBasic>();

            CreateMap<EmployeeCategoryDisplay, EmployeeCategory>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<EmployeeCategory, EmployeeCategoryDisplay>();
        }
    }
}
