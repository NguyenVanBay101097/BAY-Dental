using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerSourceProfile : Profile
    {
        public PartnerSourceProfile()
        {
            CreateMap<PartnerSource, PartnerSourceBasic>();
            CreateMap<PartnerSource, PartnerSourceSave>();
            CreateMap<PartnerSourceSave, PartnerSource>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
