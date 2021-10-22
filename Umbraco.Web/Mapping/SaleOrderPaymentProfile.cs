using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
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
                .ForMember(x => x.SaleOrderLines, x => x.MapFrom(s => s.Lines.Select(m => m.SaleOrderLine)))
                .ForMember(x => x.Payments, x => x.MapFrom(s => s.PaymentRels.Select(m => m.Payment)));

            CreateMap<SaleOrderPayment, SaleOrderPaymentSave>();

            CreateMap<SaleOrderPayment, SaleOrderPaymentDisplay>();
            CreateMap<SaleOrderPayment, SaleOrderPaymentPrintVM>()
                .ForMember(x => x.DatePayment, x => x.MapFrom(s => s.Date))
                .ForMember(x => x.JournalName, x => x.MapFrom(s => string.Join(", ", s.JournalLines.Select(c => c.Journal.Name))));

            CreateMap<SaleOrderPayment, SaleOrderPaymentPrintTemplate>()
               .ForMember(x => x.DatePayment, x => x.MapFrom(s => s.Date))
               .ForMember(x => x.User, x => x.MapFrom(s => s.CreatedBy))
               .ForMember(x => x.JournalName, x => x.MapFrom(s => string.Join(", ", s.JournalLines.Select(c => c.Journal.Name))));

            CreateMap<SaleOrderPaymentSave, SaleOrderPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore())
                .ForMember(x => x.JournalLines, x => x.MapFrom(s => s.JournalLines.Where(c => c.Amount > 0)));

            CreateMap<SaleOrderPaymentDisplay, SaleOrderPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Lines, x => x.Ignore());

            CreateMap<SaleOrderPayment, SaleOrderPaymentBasicPrintTemplate>()
                .ForMember(x => x.Payments, x => x.MapFrom(s => s.PaymentRels.Select(m => m.Payment)));

           
        }
    }
}
