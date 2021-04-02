using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AdvisoryProfile : Profile
    {
        public AdvisoryProfile()
        {

            CreateMap<Advisory, AdvisorySave>();
            CreateMap<AdvisorySave, Advisory>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<Advisory, AdvisoryDisplay>()
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.AdvisoryToothRels.Select(m => m.Tooth)))
                .ForMember(x => x.ToothDiagnosis, x => x.MapFrom(s => s.AdvisoryToothDiagnosisRels.Select(m => m.ToothDiagnosis)))
                .ForMember(x => x.Product, x => x.MapFrom(s => s.AdvisoryProductRels.Select(m => m.Product)));
            CreateMap<Advisory, AdvisoryBasic>()
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.AdvisoryToothRels.Select(m => m.Tooth)))
                .ForMember(x => x.ToothDiagnosis, x => x.MapFrom(s => s.AdvisoryToothDiagnosisRels.Select(m => m.ToothDiagnosis)))
                .ForMember(x => x.Product, x => x.MapFrom(s => s.AdvisoryProductRels.Select(m => m.Product)));
        }
    }
}
