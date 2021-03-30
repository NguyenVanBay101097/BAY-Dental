using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ToothDiagnosisProfile : Profile
    {
        public ToothDiagnosisProfile()
        {
            CreateMap<ToothDiagnosis, ToothDiagnosisBasic>();
            CreateMap<ToothDiagnosis, ToothDiagnosisSave>();
            CreateMap<ToothDiagnosisSave, ToothDiagnosis>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
