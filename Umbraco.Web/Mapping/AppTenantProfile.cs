using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AppTenantProfile : Profile
    {
        public AppTenantProfile()
        {
            CreateMap<TenantDisplay, AppTenant>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<TenantRegisterViewModel, AppTenant>();
            CreateMap<AppTenant, TenantBasic>();
        }
    }
}
