using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ResInsuranceProfile : Profile
    {
        public ResInsuranceProfile()
        {
            CreateMap<ResInsurance, ResInsuranceBasic>();

            CreateMap<ResInsurance, ResInsuranceDisplay>();

            CreateMap<ResInsurance, ResInsuranceSave>();

            CreateMap<ResInsuranceSave, ResInsurance>()
          .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<ResInsuranceDisplay, ResInsurance>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<ResInsurance, ResInsuranceSimple>();
        }
    }
}
