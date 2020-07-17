using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountAccountProfile : Profile
    {
        public AccountAccountProfile()
        {
            CreateMap<AccountAccount, AccountAccountSimple>();

            CreateMap<AccountAccount, AccountAccountBasic>();

            CreateMap<AccountAccount, AccountAccountSave>();

            CreateMap<AccountAccountSave, AccountAccount>()
                 .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.Company, x => x.Ignore())
                .ForMember(x => x.UserType, x => x.Ignore());
        }
    }
}
