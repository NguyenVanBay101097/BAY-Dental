using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Employee, x => x.Ignore());

            CreateMap<ApplicationUser, ApplicationUserDisplay>()
                .ForMember(x => x.Companies, x => x.MapFrom(s => s.ResCompanyUsersRels.Select(m => m.Company)))
                .ForMember(x => x.Groups, x => x.MapFrom(s => s.ResGroupsUsersRels.Select(m => m.Group).Where(k => !k.CategoryId.HasValue)));

            CreateMap<ApplicationUserRowExcel, ApplicationUser>();
        }
    }
}
