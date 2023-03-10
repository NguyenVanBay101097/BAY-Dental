using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
   public class SmsThanksCustomerAutomationConfigProfile : Profile
    {
        public SmsThanksCustomerAutomationConfigProfile()
        {
            CreateMap<SmsThanksCustomerAutomationConfigSave, SmsThanksCustomerAutomationConfig>();
            CreateMap<SmsThanksCustomerAutomationConfig, SmsThanksCustomerAutomationConfigDisplay>();
        }
    }
}
