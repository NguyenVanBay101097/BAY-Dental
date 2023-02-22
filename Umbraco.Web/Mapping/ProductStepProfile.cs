using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductStepProfile : Profile
    {
        public ProductStepProfile()
        {            
            CreateMap<ProductStep, ProductStepSimple>();

            CreateMap<ProductStepDisplay, ProductStep>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Product, x => x.Ignore());
            CreateMap<ProductStep, ProductStepDisplay>();
        }
    }
}
