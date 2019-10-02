using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentBasic>();

            CreateMap<Appointment, AppointmentDisplay>();
            CreateMap<AppointmentDisplay, Appointment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore());

            CreateMap<Appointment, AppointmentPatch>();
            CreateMap<AppointmentPatch, Appointment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore());
        }
    }
}
