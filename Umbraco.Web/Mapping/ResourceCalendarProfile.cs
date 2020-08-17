using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResourceCalendarProfile: Profile
    {
        public ResourceCalendarProfile()
        {
            CreateMap<ResourceCalendar, ResourceCalendarDisplay>();
            CreateMap<ResourceCalendarDisplay, ResourceCalendar>().ForMember(x => x.Id, x => x.Ignore());
            CreateMap<ResourceCalendarSave, ResourceCalendar>().ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
