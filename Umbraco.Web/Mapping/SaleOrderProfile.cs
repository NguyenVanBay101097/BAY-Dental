using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderProfile : Profile
    {
        public SaleOrderProfile()
        {
            CreateMap<SaleOrder, SaleOrderBasic>().ReverseMap();
            CreateMap<SaleOrder, SaleOrderDisplayVm>();

            CreateMap<SaleOrder, SaleOrderViewModel>();

            CreateMap<SaleOrder, SaleOrderDisplay>()
                .ForMember(x => x.OrderLines, x => x.Ignore())
                .ForMember(x => x.AmountDiscountTotal, x => x.MapFrom(s => Math.Round(s.OrderLines.Sum(z => ((decimal)z.AmountDiscountTotal) * z.ProductUOMQty))));
            CreateMap<SaleOrderDisplay, SaleOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.InvoiceStatus, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Pricelist, x => x.Ignore())
                .ForMember(x => x.Card, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.User, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore())
                .ForMember(x => x.Quote, x => x.Ignore())
                .ForMember(x => x.QuoteId, x => x.Ignore())
                .ForMember(x => x.Order, x => x.Ignore())
                .ForMember(x => x.OrderId, x => x.Ignore());

            CreateMap<SaleOrderSave, SaleOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore());

            CreateMap<SaleOrder, SaleOrderPrintVM>();
            CreateMap<SaleOrder, SaleOrderSurveyBasic>();
            CreateMap<SaleOrder, SaleOrderSimple>();
            CreateMap<SaleOrder, SaleOrderRevenueReport>();
        }
    }
}
