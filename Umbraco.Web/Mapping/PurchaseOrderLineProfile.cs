using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PurchaseOrderLineProfile : Profile
    {
        public PurchaseOrderLineProfile()
        {
            CreateMap<PurchaseOrderLine, PurchaseOrderLineDisplay>();
            CreateMap<PurchaseOrderLine, PurchaseOrderLineSave>();
            CreateMap<PurchaseOrderLineDisplay, PurchaseOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.ProductUOM, x => x.Ignore())
                .ForMember(x => x.PriceSubtotal, x => x.Ignore())
                .ForMember(x => x.PriceTotal, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.ProductId, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.ProductQty, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.PriceUnit, x => x.Condition(s => s.State == "draft"));

            CreateMap<PurchaseOrderLineSave, PurchaseOrderLine>()
              .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
