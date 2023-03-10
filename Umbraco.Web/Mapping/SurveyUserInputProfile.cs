using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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

            CreateMap<SurveyUserInput, SurveyUserInputDisplay>()
                 .ForMember(x => x.SurveyTags, x => x.MapFrom(s => s.SurveyUserInputSurveyTagRels.Select(m => m.SurveyTag)));

            CreateMap<SurveyUserInputDisplay, SurveyUserInput>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore()) ;

            CreateMap<SurveyUserInputSave, SurveyUserInput>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
