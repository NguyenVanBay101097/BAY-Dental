using AutoMapper;
using ApplicationCore.Entities;
using Umbraco.Web.Models.ContentEditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Web.Mapping
{
    public class SmsMessageDetailProfile: Profile
    {
        public SmsMessageDetailProfile()
        {
            CreateMap<SmsMessageDetail, SmsMessageDetailBasic>();
        }
    }
}
