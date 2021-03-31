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
                .ForMember(x => x.QuotationLineToothRels, x => x.MapFrom(s => s.QuotationLineToothRels.Select(m => m.Tooth)));
            CreateMap<QuotationLineSave, QuotationLine>();
        }
    }
}
