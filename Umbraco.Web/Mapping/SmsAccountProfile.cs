using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SmsAccountProfile : Profile
    {
        public SmsAccountProfile()
        {
            CreateMap<SmsAccountSave, SmsAccount>();
            CreateMap<SmsAccount, SmsAccountBasic>();
            CreateMap<SmsAccount, SmsAccountDisplay>();
        }
    }
}
