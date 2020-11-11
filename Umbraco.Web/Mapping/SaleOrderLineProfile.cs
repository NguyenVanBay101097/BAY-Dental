using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderLineProfile : Profile
    {
        public SaleOrderLineProfile()
        {
            //CreateMap<SaleOrderLine, SaleOrderLineBasic>().ReverseMap();

            CreateMap<SaleOrderLine, SaleOrderLineDisplay>()
                 .ForMember(x => x.Teeth, x => x.MapFrom(s => s.SaleOrderLineToothRels.Select(m => m.Tooth)));


            CreateMap<SaleOrderLineDisplay, SaleOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.InvoiceStatus, x => x.Ignore())
                .ForMember(x => x.PriceSubTotal, x => x.Ignore())
                .ForMember(x => x.PriceTotal, x => x.Ignore())
                .ForMember(x => x.PriceTax, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.OrderPartner, x => x.Ignore())
                .ForMember(x => x.PromotionProgramId, x => x.Ignore())
                .ForMember(x => x.PromotionId, x => x.Ignore())
                .ForMember(x => x.CouponId, x => x.Ignore())
                .ForMember(x => x.ToothCategory, x => x.Ignore())
                .ForMember(x => x.SaleOrderLineToothRels, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Salesman, x => x.Ignore())
                .ForMember(x => x.ProductId, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.ProductUOMQty, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.PriceUnit, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.ToothCategoryId, x => x.Condition(s => s.State == "draft"));

            CreateMap<SaleOrderLineSave, SaleOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.PriceUnit, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.Discount, x => x.Condition(s => s.State == "draft"))
                .ForMember(x => x.ProductId, x => x.Condition(s => s.State == "draft"));

            CreateMap<SaleOrderLine, SaleOrderLinePrintVM>();

            CreateMap<SaleOrderLine, SaleOrderLineBasic>()
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.SaleOrderLineToothRels.Select(m => m.Tooth)));
        }
    }
}
