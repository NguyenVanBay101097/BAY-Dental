using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductRequestProfile : Profile
    {
        public ProductRequestProfile()
        {
            CreateMap<ProductRequest, ProductRequestBasic>();

            CreateMap<ProductRequest, ProductRequestDisplay>();

            CreateMap<ProductRequest, ProductRequestSave>();

            CreateMap<ProductRequestDisplay, ProductRequest>()
                 .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<ProductRequestSave, ProductRequest>()
              .ForMember(x => x.Id, x => x.Ignore())
              .ForMember(x => x.Lines, x => x.Ignore());


        }
    }
}
