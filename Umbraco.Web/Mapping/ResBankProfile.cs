using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResBankProfile : Profile
    {
        public ResBankProfile()
        {
            CreateMap<ResBank,ResBankBasic>();
            CreateMap<ResBankBasic, ResBank>()
                .ForMember(x=>x.Id, x=>x.Ignore());

            CreateMap<ResBank, ResBankSimple>();
            CreateMap<ResBankSimple, ResBank>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
