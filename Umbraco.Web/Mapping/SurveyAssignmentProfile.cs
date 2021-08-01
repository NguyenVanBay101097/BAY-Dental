using ApplicationCore.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SurveyAssignmentProfile: Profile
    {
        public SurveyAssignmentProfile()
        {
            CreateMap<SurveyAssignment, SurveyAssignmentGridItem>()
                .ForMember(x => x.SurveyTags, x => x.MapFrom(s => s.UserInput.SurveyUserInputSurveyTagRels.Select(x=>x.SurveyTag.Name).Join(",")));
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
