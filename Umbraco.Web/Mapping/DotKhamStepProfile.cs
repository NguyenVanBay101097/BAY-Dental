using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class DotKhamStepProfile : Profile
    {
        public DotKhamStepProfile()
        {
            CreateMap<DotKhamStep, DotKhamStepSimple>();
            CreateMap<DotKhamStep, DotKhamStepBasic>();

            CreateMap<DotKhamStepDisplay, DotKhamStep>()
                .ForMember(x => x.DotKham, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Invoice, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore());
            CreateMap<DotKhamStep, DotKhamStepDisplay>();

            CreateMap<DotKhamStep, DotKhamStepSave>();
            CreateMap<DotKhamStepSave, DotKhamStep>()
                .ForMember(x => x.DotKham, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Invoice, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore());
        }
    }
}
