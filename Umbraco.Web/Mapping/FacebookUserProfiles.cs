using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
   public class FacebookUserProfiles : Profile
    {
        public FacebookUserProfiles()
        {
            CreateMap<FacebookUserProfile, FacebookUserProfileBasic>()
                     .ForMember(x => x.Tags, x => x.MapFrom(s => s.TagRels.Select(m => m.Tag)));

            CreateMap<FacebookUserProfile, FacebookUserProfileSave>()
            .ForMember(x => x.TagIds, x => x.MapFrom(s => s.TagRels.Select(m => m.Tag)));

            CreateMap<FacebookUserProfileSave, FacebookUserProfile>()
                 .ForMember(x => x.Id, x => x.Ignore())
                 .ForMember(x => x.FbPage, x => x.Ignore())
                 .ForMember(x => x.Partner, x => x.Ignore());
        }
    }
}
