using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class HistoriesProfile : Profile
    {
        public HistoriesProfile()
        {
            CreateMap<History, HistorySimple>();
            CreateMap<HistorySimple, History>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<HistorySimple, PartnerHistoryRel>();
        }

    }
}
