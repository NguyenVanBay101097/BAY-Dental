using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderPromotionLineProfile : Profile
    {
        public SaleOrderPromotionLineProfile()
        {
            CreateMap<SaleOrderPromotionLine, SaleOrderPromotionLineBasic>();

            CreateMap<SaleOrderPromotionLine, SaleOrderPromotionLineDisplay>();

            CreateMap<SaleOrderPromotionLine, SaleOrderPromotionLineSave>();

            CreateMap<SaleOrderPromotionLineSave, SaleOrderPromotionLine>()
          .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SaleOrderPromotionLineDisplay, SaleOrderPromotionLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
