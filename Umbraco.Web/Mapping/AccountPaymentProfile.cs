using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountPaymentProfile : Profile
    {
        public AccountPaymentProfile()
        {
            CreateMap<AccountPayment, AccountPaymentBasic>();
            CreateMap<AccountPayment, AccountPaymentDisplay>();
            CreateMap<AccountPayment, AccountPaymentViewModel>();
            CreateMap<AccountPaymentSave, AccountPayment>()
                .ForMember(x => x.SaleOrderLinePaymentRels, x => x.Ignore());

            CreateMap<AccountPayment, AccountPaymentDisplayPrintTemplate>();

            CreateMap<AccountPayment, AccountPaymentBasicPrintTemplate>();

            CreateMap<AccountPayment, AccountPaymentPrintTemplate>();
        }
    }
}
