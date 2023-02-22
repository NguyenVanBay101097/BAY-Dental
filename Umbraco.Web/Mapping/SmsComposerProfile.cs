using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models;

namespace Umbraco.Web.Mapping
{
    public class SmsComposerProfile : Profile
    {
        public SmsComposerProfile()
        {
            CreateMap<SmsComposerSave, SmsComposer>();
        }
    }
}
