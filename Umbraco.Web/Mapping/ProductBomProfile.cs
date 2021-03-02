using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductBomProfile : Profile
    {
        public ProductBomProfile()
        {
            CreateMap<ProductBom, ProductBomBasic>();

            CreateMap<ProductBom, ProductBomDisplay>();
            CreateMap<ProductBom, ProductBomSave>();

            CreateMap<ProductBomSave, ProductBom>()
                .ForMember(x => x.Id, x => x.Ignore());
            CreateMap<ProductBomDisplay, ProductBom>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }   
}
