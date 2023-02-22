using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class QuotationPromotionLineProfile : Profile
    {
        public QuotationPromotionLineProfile()
        {
            CreateMap<QuotationPromotionLine, QuotationPromotionLineBasic>();

            CreateMap<QuotationPromotionLine, QuotationPromotionLineDisplay>();

            CreateMap<QuotationPromotionLine, QuotationPromotionLineSave>();

            CreateMap<QuotationPromotionLineSave, QuotationPromotionLine>()
          .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<QuotationPromotionLineDisplay, QuotationPromotionLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
