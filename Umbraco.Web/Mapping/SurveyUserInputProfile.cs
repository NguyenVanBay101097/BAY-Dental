using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyUserInputProfile : Profile
    {
        public SurveyUserInputProfile()
        {

            CreateMap<SurveyUserInput, SurveyUserInputBasic>();

            CreateMap<SurveyUserInput, SurveyUserInputSave>();

            CreateMap<SurveyUserInput, SurveyUserInputDisplay>();

            CreateMap<SurveyUserInputDisplay, SurveyUserInput>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<SurveyUserInputSave, SurveyUserInput>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
