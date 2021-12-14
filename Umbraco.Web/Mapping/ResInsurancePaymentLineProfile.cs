using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResInsurancePaymentLineProfile : Profile
    {
        public ResInsurancePaymentLineProfile()
        {
            CreateMap<ResInsurancePaymentLine, ResInsurancePaymentLineSave>();

            CreateMap<ResInsurancePaymentLineSave, ResInsurancePaymentLine>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<ResInsurancePaymentLine, ResInsurancePaymentLineDisplay>();

            CreateMap<ResInsurancePaymentLineDisplay, ResInsurancePaymentLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
