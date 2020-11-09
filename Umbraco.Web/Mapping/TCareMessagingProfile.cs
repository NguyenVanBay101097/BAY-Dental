using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareMessagingProfile : Profile
    {
        public TCareMessagingProfile()
        {
            CreateMap<TCareMessaging, TCareMessagingBasic>()
                .ForMember(x => x.ScenarioName, o => o.MapFrom(s => s.TCareCampaign.TCareScenario.Name))
                .ForMember(x => x.CampaignName, o => o.MapFrom(s => s.TCareCampaign.Name))
                .ForMember(x => x.ScheduleDate, o => o.MapFrom(s => s.ScheduleDate.Value))
                .ForMember(x => x.PartnerTotal, o => o.MapFrom(s => s.PartnerRecipients.Count()))
                .ForMember(x => x.MessageTotal, o => o.MapFrom(s => s.TCareMessages.Count()))
                .ForMember(x => x.MessageSentTotal, o => o.MapFrom(s => s.TCareMessages.Where(s => s.Sent.HasValue && s.State == "sent").Count()))
                .ForMember(x => x.MessageExceptionTotal, o => o.MapFrom(s => s.TCareMessages.Where(s => s.State == "exception").Count()));

            CreateMap<TCareMessaging, TCareMessagingSave>();
            CreateMap<TCareMessagingSave, TCareMessaging>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<TCareMessaging, TCareMessagingDisplay>();
            CreateMap<TCareMessagingDisplay, TCareMessaging>();
        }

    }
}
