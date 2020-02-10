using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductBasic>().ReverseMap();
            CreateMap<Product, ProductDisplay>();

            CreateMap<ProductDisplay, Product>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Categ, x => x.Ignore())
                .ForMember(x => x.UOM, x => x.Ignore())
                .ForMember(x => x.UOMPO, x => x.Ignore());

            CreateMap<Product, ProductSimple>();

            CreateMap<Product, ProductLaboBasic>();
            CreateMap<Product, ProductLaboDisplay>();
            CreateMap<ProductLaboSave, Product>();
        }
    }
}
