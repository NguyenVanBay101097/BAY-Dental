using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<PurchaseOrder, PurchaseOrderBasic>();
            CreateMap<PurchaseOrder, PurchaseOrderDisplay>();
            CreateMap<PurchaseOrder, PurchaseOrderSave>();
            CreateMap<PurchaseOrderDisplay, PurchaseOrder>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.AmountTotal, x => x.Ignore())
                .ForMember(x => x.State, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.OrderLines, x => x.Ignore());

            CreateMap<PurchaseOrderSave, PurchaseOrder>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.OrderLines, x => x.Ignore());

            CreateMap<PurchaseOrder, PurchaseOrderPrintVm>();

            CreateMap<PurchaseOrder, PurchaseOrderPrintTemplate>()
                .ForMember(x => x.StockPickingName, x => x.MapFrom(s => s.Picking.Name));
        }
    }
}
