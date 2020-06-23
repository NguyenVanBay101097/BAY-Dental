using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class TCareMessagingProfile : Profile
    {
        public TCareMessagingProfile()
        {
            CreateMap<TCareMessaging, TCareMessagingBasic>();
            CreateMap<TCareMessaging, TCareMessagingSave>();
            CreateMap<TCareMessagingSave, TCareMessaging>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<TCareMessaging, TCareMessagingDisplay>();
            CreateMap<TCareMessagingDisplay, TCareMessaging>();
        }
           
    }
}
