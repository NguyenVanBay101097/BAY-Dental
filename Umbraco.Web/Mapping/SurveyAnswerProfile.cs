using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyAnswerProfile: Profile
    {
        public SurveyAnswerProfile()
        {
            CreateMap<SurveyAnswer, SurveyAnswerBasic>();
            CreateMap<SurveyAnswer, SurveyAnswerDisplay>();
            CreateMap<SurveyAnswerDisplay, SurveyAnswer>()
                .ForMember(x=> x.Id, x=> x.Ignore());
        }
    }
}
