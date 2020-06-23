using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareRuleProfile : Profile
    {
        public TCareRuleProfile()
        {
            CreateMap<TCareRule, TCareRuleBasic>();
            CreateMap<TCareRule, TCareRuleBirthdayDisplay>();
            CreateMap<TCareRuleSave, TCareRule>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Properties, x => x.Ignore());
        }
    }
}
