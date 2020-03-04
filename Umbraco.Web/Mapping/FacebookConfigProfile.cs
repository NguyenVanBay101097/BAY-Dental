using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookConfigProfile : Profile
    {
        public FacebookConfigProfile()
        {
            CreateMap<FacebookConfigSave, FacebookConfig>()
                .ForMember(x => x.ConfigPages, x => x.Ignore());

            CreateMap<FacebookConfig, FacebookConfigBasic>();
            CreateMap<FacebookConfig, FacebookConfigDisplay>();
        }
    }
}
