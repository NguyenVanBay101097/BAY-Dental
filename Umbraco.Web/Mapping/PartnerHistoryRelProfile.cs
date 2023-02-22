using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PartnerHistoryRelProfile : Profile
    {
        public PartnerHistoryRelProfile()
        {
            CreateMap<HistorySimple, PartnerHistoryRel>()
                .ForMember(x => x.HistoryId, x => x.MapFrom(s => s.Id));

            CreateMap<PartnerHistoryRel, HistorySimple>()
               .ForMember(x => x.Id, x => x.MapFrom(s => s.HistoryId))
               .ForMember(x => x.Name, x => x.MapFrom(s => s.History.Name));
        }        
    }
}
