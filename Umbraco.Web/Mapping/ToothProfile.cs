using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ToothProfile : Profile
    {
        public ToothProfile()
        {
            CreateMap<Tooth, ToothBasic>();

            CreateMap<Tooth, ToothDisplay>();
            CreateMap<ToothDisplay, Tooth>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Category, x => x.Ignore());
        }
    }
}
