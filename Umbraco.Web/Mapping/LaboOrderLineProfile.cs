using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboOrderLineProfile : Profile
    {
        public LaboOrderLineProfile()
        {
            CreateMap<LaboOrderLine, LaboOrderLineBasic>();
            CreateMap<LaboOrderLine, LaboOrderLineDisplay>()
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.LaboOrderLineToothRels.Select(m => m.Tooth)));
            CreateMap<LaboOrderLineDisplay, LaboOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.PriceSubtotal, x => x.Ignore())
                .ForMember(x => x.PriceTotal, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.ToothCategory, x => x.Ignore())
                .ForMember(x => x.Customer, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.ProductId, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.ProductQty, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.PriceUnit, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.Color, x => x.Condition(s => s.State == "draft"));

            CreateMap<LaboOrderLine, LaboOrderLinePrintVM>();
        }
    }
}
