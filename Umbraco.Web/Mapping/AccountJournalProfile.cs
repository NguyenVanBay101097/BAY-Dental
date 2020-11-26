using ApplicationCore.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Web.Models.ContentEditing;

namespace Umbraco.Web.Mapping
{
    public class AccountJournalProfile : Profile
    {
        public AccountJournalProfile()
        {
            CreateMap<AccountJournal, AccountJournalViewModel>();
            CreateMap<AccountJournal, AccountJournalSimple>();
            CreateMap<AccountJournal, AccountJournalBasic>();

            CreateMap<AccountJournalSimple, AccountJournal>();
            CreateMap<AccountJournal, AccountJournalSave>()
                .ForMember(x=>x.AccountNumber,x=>x.MapFrom(y=>y.BankAccount.AccountNumber))
                .ForMember(x=>x.BankId,x=>x.MapFrom(y=>y.BankAccount.Bank.Id));
        }
    }
}
