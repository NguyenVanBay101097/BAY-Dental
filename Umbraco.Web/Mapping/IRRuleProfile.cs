using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class IRRuleProfile : Profile
    {
        public IRRuleProfile()
        {
            CreateMap<IRRule, IRRuleBasic>();
            CreateMap<IRRule, IRRuleDisplay>();
            CreateMap<IRRuleDisplay, IRRule>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Model, x => x.Ignore());
        }
    }
}
