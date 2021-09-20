using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PrintTemplateConfigProfile : Profile
    {
        public PrintTemplateConfigProfile()
        {
            CreateMap<PrintTemplateConfig, PrintTemplateConfigDisplay>();

            CreateMap<PrintTemplateConfigDisplay, PrintTemplateConfig>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<PrintTemplateConfig, PrintTemplateConfigSave>();

            CreateMap<PrintTemplateConfigSave, PrintTemplateConfig>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
