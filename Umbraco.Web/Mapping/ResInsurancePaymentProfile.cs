using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResInsurancePaymentProfile : Profile
    {
        public ResInsurancePaymentProfile()
        {

            CreateMap<ResInsurancePayment, ResInsurancePaymentSave>();

            CreateMap<ResInsurancePayment, ResInsurancePaymentDisplay>();

            CreateMap<ResInsurancePaymentSave, ResInsurancePayment>()
              .ForMember(x => x.Id, x => x.Ignore())
              .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<ResInsurancePaymentDisplay, ResInsurancePayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
