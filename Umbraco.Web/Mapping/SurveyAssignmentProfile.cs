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
            CreateMap<SurveyAssignment, SurveyAssignmentGridItem>();
            CreateMap<SurveyAssignment, SurveyAssignmentDefaultGet>();
            CreateMap<SurveyAssignment, SurveyAssignmentSave>();

            CreateMap<SurveyAssignment, SurveyAssignmentDisplay>();

            CreateMap<SurveyAssignmentDisplay, SurveyAssignment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CallContents, x => x.Ignore());

            CreateMap<SurveyAssignmentSave, SurveyAssignment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.CallContents, x => x.Ignore());

            CreateMap<SurveyAssignment, SurveyAssignmentPatch>(); 

            CreateMap<SurveyAssignmentPatch, SurveyAssignment>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
