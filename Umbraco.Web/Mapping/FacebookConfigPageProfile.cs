using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookConfigPageProfile : Profile
    {
        public FacebookConfigPageProfile()
        {
            CreateMap<FacebookConfigPageSave, FacebookConfigPage>();
            CreateMap<FacebookConfigPage, FacebookConfigPageDisplay>();
        }
    }
}
