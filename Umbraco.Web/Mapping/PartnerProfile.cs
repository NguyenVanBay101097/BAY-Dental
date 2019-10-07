using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerProfile : Profile
    {
        public PartnerProfile()
        {
            CreateMap<Partner, PartnerSimpleContact>();
            CreateMap<Partner, PartnerBasic>().ReverseMap();

            CreateMap<Partner, PartnerDisplay>()
                .ForMember(x => x.Categories, x => x.MapFrom(s => s.PartnerPartnerCategoryRels));
            CreateMap<PartnerDisplay, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Employees, x => x.Ignore());

            CreateMap<Partner, PartnerSimple>();
            CreateMap<Partner, PartnerInfoViewModel>();

            CreateMap<Partner, PartnerPatch>();
            CreateMap<PartnerPatch, Partner>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Employees, x => x.Ignore());
        }
    }
}
