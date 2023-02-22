using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerCategoryProfile : Profile
    {
        public PartnerCategoryProfile()
        {
            CreateMap<PartnerCategory, PartnerCategoryBasic>();

            CreateMap<PartnerCategory, PartnerCategoryDisplay>();
            CreateMap<PartnerCategoryDisplay, PartnerCategory>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Parent, x => x.Ignore());
        }
    }
}
