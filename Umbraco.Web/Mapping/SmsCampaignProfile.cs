using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SmsCampaignProfile : Profile
    {
        public SmsCampaignProfile()
        {
            CreateMap<SmsCampaign, SmsCampaignBasic>();
            CreateMap<SmsCampaign, SmsCampaignSimple>();
            CreateMap<SmsCampaignSave, SmsCampaign>();
            CreateMap<SmsCampaignUpdateVM, SmsCampaign>();
        }
    }
}
