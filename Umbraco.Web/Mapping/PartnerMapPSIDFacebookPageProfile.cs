using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerMapPSIDFacebookPageProfile : Profile
    {
        public PartnerMapPSIDFacebookPageProfile()
        {
            CreateMap<PartnerMapPSIDFacebookPage, PartnerMapPSIDFacebookPageBasic>();

            CreateMap<PartnerMapPSIDFacebookPageSave, PartnerMapPSIDFacebookPage>()
                 .ForMember(x => x.Id, x => x.Ignore())
                 .ForMember(x => x.Partner, x => x.Ignore());
            CreateMap<PartnerMapPSIDFacebookPage, PartnerMapPSIDFacebookPageSave>();

            CreateMap<CheckPartnerMapFBPage, PartnerMapPSIDFacebookPage>()
                 .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<PartnerMapPSIDFacebookPage, CheckPartnerMapFBPage>();
        }
    }
}
