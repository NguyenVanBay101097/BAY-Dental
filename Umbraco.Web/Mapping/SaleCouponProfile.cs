using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleCouponProfile : Profile
    {
        public SaleCouponProfile()
        {
            CreateMap<SaleCoupon, SaleCouponBasic>();
            CreateMap<SaleCoupon, SaleCouponDisplay>();
        }
    }
}
