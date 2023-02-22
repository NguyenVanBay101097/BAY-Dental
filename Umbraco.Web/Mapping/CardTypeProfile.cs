using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class CardTypeProfile : Profile
    {
        public CardTypeProfile()
        {
            CreateMap<CardType, CardTypeBasic>();
            CreateMap<CardType, CardTypeDisplay>()
               .ForMember(x => x.ProductPricelistItems, x => x.MapFrom(z => z.Pricelist.Items));

            CreateMap<CardTypeDisplay, CardType>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Pricelist, x => x.Ignore())
                .ForMember(x => x.PricelistId, x => x.Ignore());
            CreateMap<CardTypeSave, CardType>();
        }
    }
}
