using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ServiceCardTypeProfile : Profile
    {
        public ServiceCardTypeProfile()
        {
            CreateMap<ServiceCardType, ServiceCardTypeBasic>();
            CreateMap<ServiceCardType, ServiceCardTypeDisplay>()
                .ForMember(x => x.productPricelistItems, x => x.MapFrom(z => z.ProductPricelist.Items));
            CreateMap<ServiceCardTypeSave, ServiceCardType>();
            CreateMap<CreateServiceCardTypeReq, ServiceCardType>();
            CreateMap<ServiceCardType, CreateServiceCardTypeRes>();
            CreateMap<ServiceCardType, ServiceCardTypeSimple>();
        }
    }
}
