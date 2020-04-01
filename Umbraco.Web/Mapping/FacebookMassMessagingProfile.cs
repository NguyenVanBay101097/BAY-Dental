using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookMassMessagingProfile : Profile
    {
        public FacebookMassMessagingProfile()
        {
            CreateMap<FacebookMassMessaging, FacebookMassMessagingBasic>()
                .ForMember(x => x.TotalSent, x => x.MapFrom(s => s.Traces.Where(m => m.Sent.HasValue).Count()))
                .ForMember(x => x.TotalOpened, x => x.MapFrom(s => s.Traces.Where(m => m.Opened.HasValue).Count()))
                .ForMember(x => x.TotalReceived, x => x.MapFrom(s => s.Traces.Where(m => m.Delivered.HasValue).Count()));
            CreateMap<FacebookMassMessaging, FacebookMassMessagingDisplay>()
                .ForMember(x => x.TotalSent, x => x.MapFrom(s => s.Traces.Where(m => m.Sent.HasValue).Count()))
                .ForMember(x => x.TotalOpened, x => x.MapFrom(s => s.Traces.Where(m => m.Opened.HasValue).Count()))
                .ForMember(x => x.TotalReceived, x => x.MapFrom(s => s.Traces.Where(m => m.Delivered.HasValue).Count()));
            CreateMap<FacebookMassMessagingSave, FacebookMassMessaging>();
        }
    }
}
