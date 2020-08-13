using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HrSalaryRuleProfile : Profile
    {
        public HrSalaryRuleProfile()
        {
            CreateMap<HrPayrollStructureDisplay, HrPayrollStructure>();
            CreateMap<HrPayrollStructure, HrPayrollStructureDisplay>();
        }
    }
}
