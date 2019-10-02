using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ApplicationRoleProfile : Profile
    {
        public ApplicationRoleProfile()
        {
            CreateMap<ApplicationRole, ApplicationRoleBasic>();

            CreateMap<ApplicationRole, ApplicationRoleDisplay>()
                .ForMember(x => x.Functions, x => x.MapFrom(s => s.Functions.Select(m => m.Func)));

            CreateMap<ApplicationRoleDisplay, ApplicationRole>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Functions, x => x.Ignore());
        }
    }
}
