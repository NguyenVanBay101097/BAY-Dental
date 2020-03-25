using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class CardCardProfile : Profile
    {
        public CardCardProfile()
        {
            CreateMap<CardCard, CardCardBasic>();
            CreateMap<CardCard, CardCardDisplay>();
            CreateMap<CardCardDisplay, CardCard>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.PointInPeriod, x => x.Ignore())
                .ForMember(x => x.UpgradeTypeId, x => x.Ignore())
                .ForMember(x => x.TotalPoint, x => x.Ignore())
                .ForMember(x => x.ExpiredDate, x => x.Ignore())
                .ForMember(x => x.ActivatedDate, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Type, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore());
        }
    }
}
