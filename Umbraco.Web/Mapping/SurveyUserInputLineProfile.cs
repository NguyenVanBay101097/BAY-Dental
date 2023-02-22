using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyUserInputLineProfile : Profile
    {
        public SurveyUserInputLineProfile()
        {
            CreateMap<SurveyUserInputLine, SurveyUserInputLineBasic>();

            CreateMap<SurveyUserInputLine, SurveyUserInputLineSave>();

            CreateMap<SurveyUserInputLine, SurveyUserInputLineDisplay>();

            CreateMap<SurveyUserInputLineDisplay, SurveyUserInputLine>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SurveyUserInputLineSave, SurveyUserInputLine>()
                    .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
