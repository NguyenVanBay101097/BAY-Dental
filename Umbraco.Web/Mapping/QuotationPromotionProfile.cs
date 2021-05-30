using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class QuotationPromotionProfile : Profile
    {
        public QuotationPromotionProfile()
        {
            CreateMap<QuotationPromotion, QuotationPromotionBasic>();

            CreateMap<QuotationPromotion, QuotationPromotionDisplay>();

            CreateMap<QuotationPromotion, QuotationPromotionSave>();

            CreateMap<QuotationPromotionSave, QuotationPromotion>()
          .ForMember(x => x.Id, x => x.Ignore())
          .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<QuotationPromotionDisplay, QuotationPromotion>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }

    }
}
