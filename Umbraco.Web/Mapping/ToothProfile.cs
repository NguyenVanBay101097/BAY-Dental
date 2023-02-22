using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
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
            CreateMap<Tooth, ToothSimple>();

            CreateMap<Tooth, ToothDisplay>();
            CreateMap<ToothDisplay, Tooth>()
                .ForMember(x => x.Category, x => x.Ignore());

            CreateMap<Tooth, ToothSimplePrintTemplate>();
            CreateMap<Tooth, ToothPrint>();
        }
    }
}
