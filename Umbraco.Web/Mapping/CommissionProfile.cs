using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class CommissionProfile : Profile
    {
        public CommissionProfile()
        {
            CreateMap<Commission,CommissionBasic>();

            CreateMap<Commission, CommissionDisplay >();
            CreateMap<Commission, CommissionSave>();

            CreateMap<CommissionDisplay, Commission>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CommissionProductRules, x => x.Ignore());
            CreateMap<CommissionSave, Commission>();
        }
    }
}
