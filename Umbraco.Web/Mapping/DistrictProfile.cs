using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class DistrictProfile : Profile
    {
        public DistrictProfile()
        {
            CreateMap<District, DistrictBasic>().ReverseMap();

            CreateMap<District, DistrictDisplay>();
            CreateMap<DistrictDisplay, District>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Province, x => x.Ignore());
        }
    }
}
