using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResourceCalendarAttendanceProfile : Profile
    {
        public ResourceCalendarAttendanceProfile()
        {
            CreateMap<ResourceCalendarAttendance, ResourceCalendarAttendanceBasic>();
            CreateMap<ResourceCalendarAttendance, ResourceCalendarAttendanceDisplay>();
            CreateMap<ResourceCalendarAttendanceSave, ResourceCalendarAttendance>();
        }
    }
}
