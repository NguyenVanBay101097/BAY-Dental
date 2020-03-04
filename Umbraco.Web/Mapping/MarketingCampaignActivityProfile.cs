using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class MarketingCampaignActivityProfile : Profile
    {
        public MarketingCampaignActivityProfile()
        {
            CreateMap<MarketingCampaignActivity, MarketingCampaignActivityDisplay>();

            CreateMap<MarketingCampaignActivitySave, MarketingCampaignActivity>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
