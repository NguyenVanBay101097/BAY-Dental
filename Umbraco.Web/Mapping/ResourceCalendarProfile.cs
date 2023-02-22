using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResourceCalendarProfile : Profile
    {
        public ResourceCalendarProfile()
        {
            CreateMap<ResourceCalendar, ResourceCalendarBasic>();
            CreateMap<ResourceCalendar, ResourceCalendarDisplay>();

            CreateMap<ResourceCalendar, ResourceCalendarSave>();

            CreateMap<ResourceCalendarSave, ResourceCalendar>()
               .ForMember(x => x.Id, x => x.Ignore())
               .ForMember(x => x.Attendances, x => x.Ignore())
               .ForMember(x => x.Leaves, x => x.Ignore());
        }
    }
}
