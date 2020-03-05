using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookPageProfile : Profile
    {
        public FacebookPageProfile()
        {
            CreateMap<FacebookPage, FacebookPageBasic>();
            CreateMap<FacebookPage, FacebookPageSave>();
            CreateMap<FacebookPageSave, FacebookPage>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<FacebookPageLinkSave, FacebookPage>();
        }
    }
}
