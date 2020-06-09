using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductBasic>().ReverseMap();

            CreateMap<Product, ProductServiceExportExcel>()
             .ForMember(x => x.StepList, x => x.MapFrom(s => s.Steps));
            CreateMap<Product, ProductDisplay>()
                 .ForMember(x => x.StepList, x => x.MapFrom(s => s.Steps));
            CreateMap<ProductDisplay, Product>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Categ, x => x.Ignore())
                .ForMember(x => x.UOM, x => x.Ignore())
                .ForMember(x => x.UOMPO, x => x.Ignore());
            

            CreateMap<Product, ProductSimple>();

            CreateMap<Product, ProductLaboBasic>();
            CreateMap<Product, ProductUoMBasic>().ForMember(x => x.UoMs, x => x.MapFrom(m => m.ProductUoMRels.Select(s => s.UoM)));
            CreateMap<Product, ProductLaboDisplay>();
            CreateMap<ProductLaboSave, Product>();

            //Thắng
            CreateMap<ProductSave, Product>();
  
        }
    }
}
