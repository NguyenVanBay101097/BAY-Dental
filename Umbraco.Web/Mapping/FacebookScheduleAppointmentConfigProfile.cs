using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class FacebookScheduleAppointmentConfigProfile : Profile
    {
        public FacebookScheduleAppointmentConfigProfile()
        {
            CreateMap<FacebookScheduleAppointmentConfig, FacebookScheduleAppointmentConfigBasic>();
            CreateMap<FacebookScheduleAppointmentConfig, FacebookScheduleAppointmentConfigSave>();
            CreateMap<FacebookScheduleAppointmentConfigSave, FacebookScheduleAppointmentConfig>()
                .ForMember(x => x.Id, x => x.Ignore());

        }

    }
}
