using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<Appointment, AppointmentBasic>();

            CreateMap<Appointment, AppointmentDisplay>()
                .ForMember(x => x.Services, x => x.MapFrom(s => s.AppointmentServices.Select(q => q.Product)));
            CreateMap<AppointmentDisplay, Appointment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore())
                .ForMember(x => x.AppointmentServices, x => x.Ignore());

            CreateMap<Appointment, AppointmentPatch>();
            CreateMap<AppointmentPatch, Appointment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore());

            CreateMap<Appointment, AppointmentStatePatch>();
            CreateMap<AppointmentStatePatch, Appointment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Doctor, x => x.Ignore());

            CreateMap<AppointmentPatch, AppointmentBasic>();
        }
    }
}
