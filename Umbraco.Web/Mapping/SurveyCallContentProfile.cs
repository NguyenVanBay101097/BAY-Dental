using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyCallContentProfile : Profile
    {
        public SurveyCallContentProfile()
        {
            CreateMap<SurveyCallContent, SurveyCallContentBasic>();

            CreateMap<SurveyCallContent, SurveyCallContentSave>();

            CreateMap<SurveyCallContent, SurveyCallContentDisplay>();

            CreateMap<SurveyCallContentDisplay, SurveyCallContent>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SurveyCallContentSave, SurveyCallContent>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
