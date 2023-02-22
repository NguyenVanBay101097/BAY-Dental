using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductRequestLineProfile : Profile
    {
        public ProductRequestLineProfile()
        {
            CreateMap<ProductRequestLine, ProductRequestLineBasic>();

            CreateMap<ProductRequestLine, ProductRequestLineDisplay>();

            CreateMap<ProductRequestLine, ProductRequestLineSave>();

            CreateMap<ProductRequestLineDisplay, ProductRequestLine>()
                 .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<ProductRequestLineSave, ProductRequestLine>()
                  .ForMember(x => x.Id, x => x.Ignore());
        }
      
    }
}
