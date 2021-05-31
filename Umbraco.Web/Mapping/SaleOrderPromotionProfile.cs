using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderPromotionProfile : Profile
    {
        public SaleOrderPromotionProfile()
        {
            CreateMap<SaleOrderPromotion, SaleOrderPromotionBasic>();

            CreateMap<SaleOrderPromotion, SaleOrderPromotionDisplay>();

            CreateMap<SaleOrderPromotion, SaleOrderPromotionSave>();

            CreateMap<SaleOrderPromotionSave, SaleOrderPromotion>()
          .ForMember(x => x.Id, x => x.Ignore())
          .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<SaleOrderPromotionDisplay, SaleOrderPromotion>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
