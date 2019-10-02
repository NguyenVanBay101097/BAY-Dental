using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProvinceProfile : Profile
    {
        public ProvinceProfile()
        {
            CreateMap<Province, ProvinceBasic>().ReverseMap();

            CreateMap<Province, ProvinceDisplay>().ReverseMap();
            CreateMap<ProvinceDisplay, Province>()
              .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
