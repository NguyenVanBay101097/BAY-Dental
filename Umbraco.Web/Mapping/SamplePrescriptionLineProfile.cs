using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SamplePrescriptionLineProfile : Profile
    {
        public SamplePrescriptionLineProfile()
        {
            CreateMap<SamplePrescriptionLine, SamplePrescriptionLineBasic>();

            CreateMap<SamplePrescriptionLine, SamplePrescriptionLineSave>();

            CreateMap<SamplePrescriptionLineSave, SamplePrescriptionLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Prescription, x => x.Ignore());

            CreateMap<SamplePrescriptionLine, SamplePrescriptionLineDisplay>();

            CreateMap<SamplePrescriptionLineDisplay, SamplePrescriptionLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Prescription, x => x.Ignore());
        }
    }
}
