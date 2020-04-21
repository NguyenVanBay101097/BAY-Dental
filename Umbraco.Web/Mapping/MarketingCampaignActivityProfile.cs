using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class MarketingCampaignActivityProfile : Profile
    {
        public MarketingCampaignActivityProfile()
        {
            CreateMap<MarketingCampaignActivity, MarketingCampaignActivityBasic>()
             .ForMember(x => x.Tags, x => x.MapFrom(s => s.TagRels.Select(m => m.Tag)));

            CreateMap<MarketingCampaignActivity, MarketingCampaignActivityDisplay>()
                 .ForMember(x => x.Tags, x => x.MapFrom(s => s.TagRels.Select(m => m.Tag)));

            CreateMap<MarketingCampaignActivity, MarketingCampaignActivitySave>()
                .ForMember(x => x.TagIds, x => x.MapFrom(s => s.TagRels.Select(m => m.Tag)));

            CreateMap<MarketingCampaignActivitySave, MarketingCampaignActivity>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<MarketingCampaignActivity, MarketingCampaignActivitySimple>();
        }
    }
}
 