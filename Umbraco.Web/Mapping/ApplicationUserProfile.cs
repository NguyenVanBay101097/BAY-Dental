using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserSimple>();
            CreateMap<ApplicationUser, ApplicationUserBasic>();

            CreateMap<ApplicationUserDisplay, ApplicationUser>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<ApplicationUser, ApplicationUserDisplay>();
        }
    }
}
