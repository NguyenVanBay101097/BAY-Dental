using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyQuestionProfile: Profile
    {
        public SurveyQuestionProfile()
        {
            CreateMap<SurveyQuestion, SurveyQuestionBasic>();
            CreateMap<SurveyQuestion, SurveyQuestionDisplay>();
            CreateMap<SurveyQuestionSave, SurveyQuestion>()
                .ForMember(x => x.Answers, x => x.Ignore());
            CreateMap<SurveyQuestionUpdateListPar, SurveyQuestion>().ForMember(x=> x.Answers, x=> x.Ignore());
        }
    }
}
