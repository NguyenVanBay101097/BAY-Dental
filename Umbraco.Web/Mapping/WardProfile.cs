using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class WardProfile : Profile
    {
        public WardProfile()
        {
            CreateMap<Ward, WardBasic>().ReverseMap();

            CreateMap<Ward, WardDisplay>();
            CreateMap<WardDisplay, Ward>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.District, x => x.Ignore());
        }
    }
}
