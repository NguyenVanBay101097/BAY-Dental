using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class QuotationLineProfile : Profile
    {
        public QuotationLineProfile()
        {
            CreateMap<QuotationLine, QuotationLineBasic>();
            CreateMap<QuotationLine, QuotationLineDisplay>()
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.QuotationLineToothRels.Select(m => m.Tooth)))
                .ForMember(x => x.AmountPromotionToOrder, x => x.MapFrom(s => s.PromotionLines.Where(s => !s.Promotion.QuotationLineId.HasValue).Sum(s => s.PriceUnit)))
                .ForMember(x => x.AmountPromotionToOrderLine, x => x.MapFrom(s => s.PromotionLines.Where(s => s.Promotion.QuotationLineId.HasValue).Sum(s => s.PriceUnit)));

            CreateMap<QuotationLineSave, QuotationLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
