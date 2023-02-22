using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class MarketingCampaignProfile : Profile
    {
        public MarketingCampaignProfile()
        {
            CreateMap<MarketingCampaign, MarketingCampaignBasic>();
            CreateMap<MarketingCampaign, MarketingCampaignDisplay>();

            CreateMap<MarketingCampaignSave, MarketingCampaign>()
                .ForMember(x => x.Activities, x => x.Ignore());
        }
    }
}
