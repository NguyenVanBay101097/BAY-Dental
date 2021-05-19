using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleCouponProgramProfile : Profile
    {
        public SaleCouponProgramProfile()
        {
            CreateMap<SaleCouponProgram, SaleCouponProgramBasic>();
            CreateMap<SaleCouponProgram, SaleCouponProgramGetListPagedResponse>();
            CreateMap<SaleCouponProgram, SaleCouponProgramSimple>();
            CreateMap<SaleCouponProgramSave, SaleCouponProgram>()
                .ForMember(x => x.Days, x => x.MapFrom(s => string.Join(",", s.Days)));
            CreateMap<SaleCouponProgram, SaleCouponProgramDisplay>()
                .ForMember(x => x.DiscountSpecificProducts, x => x.MapFrom(s => s.DiscountSpecificProducts.Select(m => m.Product)))
                .ForMember(x => x.DiscountSpecificProducts, x => x.MapFrom(s => s.DiscountSpecificPartners.Select(m => m.Partner)))
                .ForMember(x => x.CouponCount, x => x.MapFrom(s => s.Coupons.Count))
                .ForMember(x => x.Days, x => x.Ignore())
                .ForMember(x => x.DiscountSpecificProductCategories, x => x.MapFrom(s => s.DiscountSpecificProductCategories.Select(m => m.ProductCategory)));
        }
    }
}
