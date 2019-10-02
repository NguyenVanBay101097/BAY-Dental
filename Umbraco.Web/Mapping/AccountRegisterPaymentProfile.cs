using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountRegisterPaymentProfile : Profile
    {
        public AccountRegisterPaymentProfile()
        {
            CreateMap<AccountRegisterPaymentDisplay, AccountRegisterPayment>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Journal, x => x.Ignore())
                .ForMember(x => x.Partner, x => x.Ignore())
                .ForMember(x => x.AccountRegisterPaymentInvoiceRels, x => x.Ignore());
        }
    }
}
