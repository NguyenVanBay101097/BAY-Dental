using ApplicationCore.Entities;
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
            CreateMap<AccountPaymentSave, AccountPayment>();
        }
    }
}
