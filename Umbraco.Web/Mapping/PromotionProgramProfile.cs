using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PromotionProgramProfile : Profile
    {
        public PromotionProgramProfile()
        {
            CreateMap<PromotionProgram, PromotionProgramBasic>();
            CreateMap<PromotionProgram, PromotionProgramDisplay>();
            CreateMap<PromotionProgramSave, PromotionProgram>()
                .ForMember(x => x.Rules, x => x.Ignore());
        }
    }
}
