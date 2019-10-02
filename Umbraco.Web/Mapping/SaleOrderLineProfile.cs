using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderLineProfile : Profile
    {
        public SaleOrderLineProfile()
        {
            //CreateMap<SaleOrderLine, SaleOrderLineBasic>().ReverseMap();

            CreateMap<SaleOrderLine, SaleOrderLineDisplay>();
            CreateMap<SaleOrderLineDisplay, SaleOrderLine>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.OrderPartner, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Salesman, x => x.Ignore());
        }
    }
}
