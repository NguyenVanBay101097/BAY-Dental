using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderPaymentProfile : Profile
    {
        public SaleOrderPaymentProfile()
        {
            CreateMap<SaleOrderPayment, SaleOrderPaymentBasic>();

            CreateMap<SaleOrderPayment, SaleOrderPaymentSave>();

            CreateMap<SaleOrderPayment, SaleOrderPaymentDisplay>();

            CreateMap<SaleOrderPaymentSave, SaleOrderPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<SaleOrderPaymentDisplay, SaleOrderPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
