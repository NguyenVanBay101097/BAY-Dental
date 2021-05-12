using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderPaymentProfile : Profile
    {
        public SaleOrderPaymentProfile()
        {
            CreateMap<SaleOrderPayment, SaleOrderPaymentBasic>()
                .ForMember(x => x.Payments, x => x.MapFrom(s => s.PaymentRels.Select(m => m.Payment)));

            CreateMap<SaleOrderPayment, SaleOrderPaymentSave>();

            CreateMap<SaleOrderPayment, SaleOrderPaymentDisplay>();
            CreateMap<SaleOrderPayment, SaleOrderPaymentPrintVM>();

            CreateMap<SaleOrderPaymentSave, SaleOrderPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<SaleOrderPaymentDisplay, SaleOrderPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());
        }
    }
}
