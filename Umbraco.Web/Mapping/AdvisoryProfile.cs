using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AdvisoryProfile : Profile
    {
        public AdvisoryProfile()
        {
            CreateMap<Advisory, AdvisoryBasic>();
            CreateMap<Advisory, AdvisorySave>();
            CreateMap<AdvisorySave, Advisory>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
