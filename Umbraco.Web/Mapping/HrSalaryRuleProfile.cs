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
            CreateMap<HrSalaryRuleDisplay, HrSalaryRule>().ForMember(x=>x.Id,x=>x.Ignore());
            CreateMap<HrSalaryRule, HrSalaryRuleDisplay>();
            CreateMap<HrSalaryRuleSave, HrSalaryRule>().ForMember(x => x.Id, x => x.Ignore());

        }
    }
}
