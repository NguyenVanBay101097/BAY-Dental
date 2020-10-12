using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareMessageTemplateProfile : Profile
    {
        public TCareMessageTemplateProfile()
        {
            CreateMap<TCareMessageTemplate, TCareMessageTemplateBasic>();
            CreateMap<TCareMessageTemplate, TCareMessageTemplateDisplay>();
            CreateMap<TCareMessageTemplateSave, TCareMessageTemplate>().ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
