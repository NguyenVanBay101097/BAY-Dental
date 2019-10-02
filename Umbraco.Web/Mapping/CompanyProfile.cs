using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyBasic>();

            CreateMap<Company, CompanyDisplay>();
            CreateMap<CompanyDisplay, Company>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
