using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerTitleProfile : Profile
    {
        public PartnerTitleProfile()
        {
            CreateMap<PartnerTitle, PartnerTitleBasic>();
            CreateMap<PartnerTitle, PartnerTitleSave>();
            CreateMap<PartnerTitleSave, PartnerTitle>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
