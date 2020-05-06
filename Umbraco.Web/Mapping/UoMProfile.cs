using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class UoMProfile : Profile
    {
        public UoMProfile()
        {
            CreateMap<UoM, UoMBasic>().ReverseMap();

            CreateMap<UoM, UoMSimple>();
            CreateMap<UoMSave, UoM>();
            CreateMap<UoM, UoMDisplay>();
        }
    }
}
