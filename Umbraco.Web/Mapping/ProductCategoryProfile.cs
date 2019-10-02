using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductCategoryProfile: Profile
    {
        public ProductCategoryProfile()
        {
            CreateMap<ProductCategory, ProductCategoryBasic>().ReverseMap();

            CreateMap<ProductCategory, ProductCategoryDisplay>();
            CreateMap<ProductCategoryDisplay, ProductCategory>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Parent, opt => opt.Ignore());

            CreateMap<ProductCategory, ProductCategorySimple>();
        }
    }
}
