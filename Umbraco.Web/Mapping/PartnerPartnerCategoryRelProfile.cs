using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerPartnerCategoryRelProfile : Profile
    {
        public PartnerPartnerCategoryRelProfile()
        {
            CreateMap<PartnerCategoryBasic, PartnerPartnerCategoryRel>()
                .ForMember(x => x.CategoryId, x => x.MapFrom(s => s.Id));

            CreateMap<PartnerPartnerCategoryRel, PartnerCategoryBasic>()
               .ForMember(x => x.Id, x => x.MapFrom(s => s.CategoryId))
               .ForMember(x => x.Color, x => x.MapFrom(s => s.Category.Color))
               .ForMember(x => x.Name, x => x.MapFrom(s => s.Category.Name))
               .ForMember(x => x.CompleteName, x => x.MapFrom(s => s.Category.CompleteName));
        }
    }
}
