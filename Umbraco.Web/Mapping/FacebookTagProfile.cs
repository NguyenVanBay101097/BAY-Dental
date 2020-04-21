using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookTagProfile : Profile
    {
        public FacebookTagProfile()
        {
            CreateMap<FacebookTag, FacebookTagBasic>();
            CreateMap<FacebookTagSave, FacebookTag>();
            CreateMap<FacebookTag, FacebookTagSimple>().ReverseMap();
            CreateMap<FacebookTagSimple, FacebookTag>()
                .ForMember(x => x.Id, x => x.Ignore());

            //CreateMap<FacebookTagSimple, FacebookUserProfileTagRel>();
        }
    }
}
