using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductPricelistProfile : Profile
    {
        public ProductPricelistProfile()
        {
            CreateMap<ProductPricelist, ProductPricelistSave>();
            CreateMap<ProductPricelistSave, ProductPricelist>()
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Items, x => x.Ignore())
                .ForMember(x => x.PartnerCateg, x => x.Ignore());

            CreateMap<ProductPricelist, ProductPricelistBasic>();
            CreateMap<ProductPricelistBasic, ProductPricelist>()
                .ForMember(x=>x.Company, x=>x.Ignore())
                .ForMember(x => x.PartnerCateg, x => x.Ignore());


            CreateMap<ProductPricelistItem, ProductPricelistItemSave>();
            CreateMap<ProductPricelistItemSave, ProductPricelistItem>()
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.Categ, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore())
                .ForMember(x => x.PartnerCateg, x => x.Ignore());
        }
    }
}
