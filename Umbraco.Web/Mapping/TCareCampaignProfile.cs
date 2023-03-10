using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareCampaignProfile : Profile
    {
        public TCareCampaignProfile()
        {
            CreateMap<TCareCampaign, TCareCampaignBasic>();
            CreateMap<TCareCampaignSave, TCareCampaign>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<TCareCampaignStart, TCareCampaign>();

        }
    }
}
