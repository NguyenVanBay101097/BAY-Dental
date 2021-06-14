using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
   public class SmsBirthdayAutomationConfigProfile : Profile
    {
        public SmsBirthdayAutomationConfigProfile()
        {
            CreateMap<SmsBirthdayAutomationConfigSave, SmsBirthdayAutomationConfig>();
            CreateMap<SmsBirthdayAutomationConfig, SmsBirthdayAutomationConfigDisplay>();
        }
    }
}
