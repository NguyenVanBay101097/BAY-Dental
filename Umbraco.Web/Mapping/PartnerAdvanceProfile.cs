using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerAdvanceProfile : Profile
    {
        public PartnerAdvanceProfile()
        {
            CreateMap<PartnerAdvance, PartnerAdvanceBasic>()
                 .ForMember(x => x.Amount, o => o.MapFrom(s => s.Type == "advance" ? s.Amount : -s.Amount)); 

            CreateMap<PartnerAdvance, PartnerAdvanceSave>();

            CreateMap<PartnerAdvance, PartnerAdvanceDisplay>();

            CreateMap<PartnerAdvanceSave, PartnerAdvance>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<PartnerAdvanceDisplay, PartnerAdvance>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
