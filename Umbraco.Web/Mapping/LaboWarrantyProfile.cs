using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class LaboWarrantyProfile : Profile
    {
        public LaboWarrantyProfile()
        {
            CreateMap<LaboWarranty, LaboWarrantyBasic>()
                .ForMember(x => x.LaboOrderId, x => x.MapFrom(s => s.LaboOrderId))
                .ForMember(x => x.LaboOrderName, x => x.MapFrom(s => s.LaboOrder.Name))
                .ForMember(x => x.CustomerId, x => x.MapFrom(s => s.LaboOrder.CustomerId))
                .ForMember(x => x.CustomerRef, x => x.MapFrom(s => s.LaboOrder.Customer.Ref))
                .ForMember(x => x.CustomerName, x => x.MapFrom(s => s.LaboOrder.Customer.Name))
                .ForMember(x => x.CustomerDisplayName, x => x.MapFrom(s => s.LaboOrder.Customer.DisplayName));

            CreateMap<LaboWarranty, LaboWarrantyDisplay>()
                .ForMember(x => x.LaboOrderId, x => x.MapFrom(s => s.LaboOrderId))
                .ForMember(x => x.LaboOrderName, x => x.MapFrom(s => s.LaboOrder.Name))
                .ForMember(x => x.SupplierId, x => x.MapFrom(s => s.LaboOrder.PartnerId))
                .ForMember(x => x.SupplierName, x => x.MapFrom(s => s.LaboOrder.Partner.Name))
                .ForMember(x => x.CustomerId, x => x.MapFrom(s => s.LaboOrder.CustomerId))
                .ForMember(x => x.CustomerRef, x => x.MapFrom(s => s.LaboOrder.Customer.Ref))
                .ForMember(x => x.CustomerName, x => x.MapFrom(s => s.LaboOrder.Customer.Name))
                .ForMember(x => x.CustomerDisplayName, x => x.MapFrom(s => s.LaboOrder.Customer.DisplayName))
                .ForMember(x => x.SaleOrderLineName, x => x.MapFrom(s => s.LaboOrder.SaleOrderLine.Name))
                .ForMember(x => x.TeethLaboOrder, x => x.MapFrom(s => s.LaboOrder.LaboOrderToothRel.Select(m => m.Tooth)))
                .ForMember(x => x.Teeth, x => x.MapFrom(s => s.LaboWarrantyToothRels.Select(m => m.Tooth)));

            CreateMap<LaboWarrantyBasic, LaboWarranty>();
            CreateMap<LaboWarrantyDisplay, LaboWarranty>();
            CreateMap<LaboWarrantySave, LaboWarranty>();
        }
    }
}
