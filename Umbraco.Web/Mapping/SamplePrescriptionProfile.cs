﻿using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SamplePrescriptionProfile : Profile
    {
        public SamplePrescriptionProfile()
        {
            CreateMap<SamplePrescription, SamplePrescriptionBasic>();

            CreateMap<SamplePrescription, SamplePrescriptionSave>();

            CreateMap<SamplePrescription, SamplePrescriptionDisplay>()
                .ForMember(x => x.Lines, x => x.Ignore()); ;

            CreateMap<SamplePrescriptionDisplay, SamplePrescription>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<SamplePrescriptionSave, SamplePrescription>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
