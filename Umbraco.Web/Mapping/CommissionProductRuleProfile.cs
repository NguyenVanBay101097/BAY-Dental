using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class CommissionProductRuleProfile : Profile
    {
        public CommissionProductRuleProfile()
        {
            CreateMap<CommissionProductRule, CommissionProductRuleBasic>();

            CreateMap<CommissionProductRule, CommissionProductRuleDisplay>();
            CreateMap<CommissionProductRuleDisplay, CommissionProductRule>()
                 .ForMember(x => x.Id, x => x.Ignore())
                 .ForMember(x => x.Product, x => x.Ignore())
                  .ForMember(x => x.Categ, x => x.Ignore());
        }
    }
}
