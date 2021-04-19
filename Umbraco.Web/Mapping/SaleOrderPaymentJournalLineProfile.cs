using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderPaymentJournalLineProfile : Profile
    {
        public SaleOrderPaymentJournalLineProfile()
        {
            CreateMap<SaleOrderPaymentJournalLine, SaleOrderPaymentJournalLineBasic>();

            CreateMap<SaleOrderPaymentJournalLine, SaleOrderPaymentJournalLineSave>();

            CreateMap<SaleOrderPaymentJournalLineSave, SaleOrderPaymentJournalLine>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SaleOrderPaymentJournalLine, SaleOrderPaymentJournalLineDisplay>();

            CreateMap<SaleOrderPaymentJournalLineDisplay, SaleOrderPaymentJournalLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
