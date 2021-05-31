using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class SaleOrderPaymentHistoryLineProfile : Profile
    {
        public SaleOrderPaymentHistoryLineProfile()
        {
            CreateMap<SaleOrderPaymentHistoryLine, SaleOrderPaymentHistoryLineBasic>();

            CreateMap<SaleOrderPaymentHistoryLine, SaleOrderPaymentHistoryLineSave>();

            CreateMap<SaleOrderPaymentHistoryLineSave, SaleOrderPaymentHistoryLine>()
                .ForMember(x => x.Id, x => x.Ignore());

            CreateMap<SaleOrderPaymentHistoryLine, SaleOrderPaymentHistoryLineDisplay>();

            CreateMap<SaleOrderPaymentHistoryLineDisplay, SaleOrderPaymentHistoryLine>()
                .ForMember(x => x.Id, x => x.Ignore());
        }
    }
}
