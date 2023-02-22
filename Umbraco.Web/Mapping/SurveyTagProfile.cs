using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyTagProfile : Profile
    {
        public SurveyTagProfile()
        {
            CreateMap<SurveyTag, SurveyTagBasic>();

            CreateMap<SurveyTag, SurveyTagSave>();
            CreateMap<SurveyTagSave, SurveyTag>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
