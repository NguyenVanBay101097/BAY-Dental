using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SmsMessageProfile : Profile
    {
        public SmsMessageProfile()
        {
            CreateMap<SmsMessage, SmsMessageDisplay>()
                .ForMember(x => x.Partners, x => x.MapFrom(s => s.SmsMessagePartnerRels.Select(m => m.Partner)));
            CreateMap<SmsMessageSave, SmsMessage>();
        }
    }
}
