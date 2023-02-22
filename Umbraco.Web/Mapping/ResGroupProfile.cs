using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResGroupProfile : Profile
    {
        public ResGroupProfile()
        {
            CreateMap<ResGroup, ResGroupBasic>();

            CreateMap<ResGroup, ResGroupDisplay>()
                .ForMember(x => x.Users, x => x.MapFrom(s => s.ResGroupsUsersRels.Select(m => m.User)));
            CreateMap<ResGroupDisplay, ResGroup>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.ModelAccesses, x => x.Ignore());
        }
    }
}
