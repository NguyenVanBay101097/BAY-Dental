using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyAssignmentProfile: Profile
    {
        public SurveyAssignmentProfile()
        {
            CreateMap<SurveyAssignment, SurveyAssignmentBasic>();
            CreateMap<SurveyAssignment, SurveyAssignmentDefaultGet>();
            CreateMap<SurveyAssignmentSave, SurveyAssignment>();
            CreateMap<SurveyAssignment, SurveyAssignmentPatch>().ForMember(x => x.Id, x => x.Ignore()); ;
            CreateMap<SurveyAssignmentPatch, SurveyAssignment>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
