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
                 .ForMember(x => x.Teeth, x => x.MapFrom(s => s.SaleOrderLineToothRels.Select(m => m.Tooth))); ;
            CreateMap<SaleOrderLineDisplay, SaleOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.InvoiceStatus, x => x.Ignore())
                .ForMember(x => x.OrderPartner, x => x.Ignore())
                .ForMember(x => x.ToothCategory, x => x.Ignore())
                .ForMember(x => x.SaleOrderLineToothRels, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Salesman, x => x.Ignore());
        }
    }
}
