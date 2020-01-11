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
            CreateMap<SaleCouponProgramSave, SaleCouponProgram>();
            CreateMap<SaleCouponProgram, SaleCouponProgramDisplay>()
                .ForMember(x => x.DiscountSpecificProducts, x => x.MapFrom(s => s.DiscountSpecificProducts.Select(m => m.Product)))
                .ForMember(x => x.CouponCount, x => x.MapFrom(s => s.Coupons.Count));
        }
    }
}
