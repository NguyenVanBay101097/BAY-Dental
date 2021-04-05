using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ConfigPrintProfile : Profile
    {
        public ConfigPrintProfile()
        {
            CreateMap<ConfigPrint, ConfigPrintBasic>();

            CreateMap<ConfigPrint, ConfigPrintSave>();

            CreateMap<ConfigPrint, ConfigPrintDisplay>();

            CreateMap<ConfigPrintSave, ConfigPrint>()
                .ForMember(x => x.Id, x => x.Ignore());

        }
    }
}
