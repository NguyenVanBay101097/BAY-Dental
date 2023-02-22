using ApplicationCore.Entities;
using ApplicationCore.Models.PrintTemplate;
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
            CreateMap<AccountJournal, AccountJournalResBankSimple>();

            CreateMap<AccountJournalSimple, AccountJournal>();
            CreateMap<AccountJournal, AccountJournalSave>()
                .ForMember(x => x.AccountNumber, x => x.MapFrom(y => y.BankAccount.AccountNumber))
                .ForMember(x => x.AccountHolderName, x => x.MapFrom(y => y.BankAccount.AccountHolderName))
                .ForMember(x => x.BankBranch, x => x.MapFrom(y => y.BankAccount.Branch))
                .ForMember(x => x.BankId, x => x.MapFrom(y => y.BankAccount.Bank.Id));
            CreateMap<AccountJournal, AccountJournalDisplay>()
                .ForMember(x => x.AccountNumber, x => x.MapFrom(y => y.BankAccount.AccountNumber))
                .ForMember(x => x.AccountHolderName, x => x.MapFrom(y => y.BankAccount.AccountHolderName))
                .ForMember(x => x.BankBranch, x => x.MapFrom(y => y.BankAccount.Branch))
                .ForMember(x => x.BankId, x => x.MapFrom(y => y.BankAccount.Bank.Id));

            CreateMap<AccountJournal, AccountJournalSimplePrintTemplate>();
        }
    }
}
