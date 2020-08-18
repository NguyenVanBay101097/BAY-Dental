using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class AccountJournalService : BaseService<AccountJournal>, IAccountJournalService
    {
        private readonly IMapper _mapper;

        public AccountJournalService(IAsyncRepository<AccountJournal> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<AccountJournal> GetJournalByTypeAndCompany(string type, Guid companyId)
        {
            return await SearchQuery(x => x.Type == type && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<AccountJournal>> CreateAsync(IEnumerable<AccountJournal> self)
        {
            foreach (var journal in self)
            {
                var seq = await _CreateSequence(journal);
                journal.SequenceId = seq.Id;

                if (journal.DedicatedRefund && (journal.Type == "sale" || journal.Type == "purchase"))
                {
                    var refund_seq = await _CreateSequence(journal, refund: true);
                    journal.RefundSequenceId = refund_seq.Id;
                }
            }

            return await base.CreateAsync(self);
        }

        public async Task CreateJournals(IEnumerable<AccountJournalSave> vals)
        {
            var accountObj = GetService<IAccountAccountService>();

            foreach (var val in vals)
            {
                var journal = new AccountJournal();

                if (val.Type == "bank" && !journal.BankAccountId.HasValue && !string.IsNullOrEmpty(val.AccountNumber))
                {
                    var partnerBank = await SetPartnerBankAsync(val.AccountNumber, val.BankId);
                    journal.BankAccountId = partnerBank.Id;
                }

                if (string.IsNullOrEmpty(journal.Code))
                {
                    //hàm phát sinh code kiểu bank hoặc cash
                    journal.Code = await GenerateNextCodeBankCashAsync(val.Type);
                    if (string.IsNullOrEmpty(journal.Code))
                        throw new Exception("Không thể phát sinh code");
                }
                var name = "";
                if (!string.IsNullOrEmpty(val.AccountNumber) && val.BankId.HasValue && string.IsNullOrEmpty(val.Name) && val.Type == "bank")
                    name = await GetBankAccountNameAsync(val.AccountNumber, val.BankId);
                else
                    name = val.Name;
                if (!journal.DefaultDebitAccountId.HasValue || !journal.DefaultCreditAccountId.HasValue)
                {
                    var account = await PrepareAccountLiquidityAsync(val.Type, name);
                    var entityAccount = await accountObj.CreateAsync(account);
                    journal.DefaultCreditAccountId = entityAccount.Id;
                    journal.DefaultDebitAccountId = entityAccount.Id;
                }

                journal.Name = name;
                journal.Type = val.Type;
                journal.CompanyId = CompanyId;
                //tạo sequence
                var seq = await _CreateSequence(journal);
                journal.SequenceId = seq.Id;

                if (journal.DedicatedRefund && (journal.Type == "sale" || journal.Type == "purchase"))
                {
                    var refund_seq = await _CreateSequence(journal, refund: true);
                    journal.RefundSequenceId = refund_seq.Id;
                }
                await base.CreateAsync(journal);
            }
        }

        public async Task UpdateJournalSave(Guid id, AccountJournalSave val)
        {
            var resPnBankObj = GetService<IResPartnerBankService>();
            var accountObj = GetService<IAccountAccountService>();

            var journal = SearchQuery(x => x.Id.Equals(id))
                .Include(x => x.BankAccount)
                .ThenInclude(x => x.Bank)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(val.AccountNumber) && val.BankId.HasValue)
                journal.Name = await GetBankAccountNameAsync(val.AccountNumber, val.BankId);

            //var resPartnerBank = await resPnBankObj.SearchQuery(x => x.Id == journal.BankAccountId).FirstOrDefaultAsync();
            //journal.Type là bank hoặc cash va journal.Code dang null thì
            //neu val.Type != journal.Type và journal.Type là bank hoac cash
            //if (val.Type != journal.Type)
            //{
            //    journal.Code = await GenerateNextCodeBankCashAsync(val.Type);


            //    if (val.Type == "cash")
            //    {
            //        await resPnBankObj.DeleteAsync(resPartnerBank);
            //        journal.BankAccountId = null;
            //    } else if (val.Type == "bank")
            //    {
            //        var rpb = await SetPartnerBankAsync(val.AccountNumber, val.BankId);
            //        journal.BankAccountId = rpb.Id;
            //        resPartnerBank = rpb;
            //    }
            //}

            //if (val.BankId.HasValue && resPartnerBank!=null && !string.IsNullOrEmpty(val.AccountNumber))
            //{
            //    resPartnerBank.BankId = val.BankId ?? Guid.Empty;
            //    resPartnerBank.AccountNumber = val.AccountNumber;
            //    if (resPartnerBank.BankId != Guid.Empty)
            //        await resPnBankObj.UpdateAsync(resPartnerBank);
            //}

            //if(journal.DefaultDebitAccountId.HasValue && journal.DefaultCreditAccountId.HasValue)
            //{
            //    var account = await accountObj.SearchQuery(x => x.Id == journal.DefaultDebitAccountId && x.Id == journal.DefaultCreditAccountId)
            //        .FirstOrDefaultAsync();
            //    account.Name = name;
            //    if (journal.Type != val.Type)
            //    {
            //        journal.Type = val.Type;
            //        var accountPrepared = await PrepareAccountLiquidityAsync(val.Type, name);
            //        account.Code = accountPrepared.Code;
            //    }


            //    await accountObj.UpdateAsync(account);
            //}                        

            await UpdateAsync(journal);
        }

        private async Task<string> GetBankAccountNameAsync(string accountNumber, Guid? bankId)
        {
            var bankObj = GetService<IResBankService>();
            var bank = await bankObj.SearchQuery(x => x.Id.Equals(bankId)).FirstOrDefaultAsync();
            return string.Format("{0} - {1}", bank.Name, accountNumber);
        }

        private async Task<ResPartnerBank> SetPartnerBankAsync(string accountNumber, Guid? bankId)
        {
            var companyObj = GetService<ICompanyService>();
            var resPnBankObj = GetService<IResPartnerBankService>();

            var company = companyObj.SearchQuery(x => x.Id.Equals(CompanyId)).FirstOrDefault();
            var partnerBank = new ResPartnerBankDisplay
            {
                AccountNumber = accountNumber,
                BankId = bankId ?? Guid.Empty,
                PartnerId = company.PartnerId
            };
            var pb = _mapper.Map<ResPartnerBank>(partnerBank);
            await resPnBankObj.CreateAsync(pb);

            return pb;
        }

        private async Task<AccountAccount> PrepareAccountLiquidityAsync(string type, string name)
        {
            var accountObj = GetService<IAccountAccountService>();
            var typeObj = GetService<IAccountAccountTypeService>();

            var prefix = type == "bank" ? "1112." : "1111.";
            var matchedCodes = await accountObj.SearchQuery(x => x.Code.Contains(prefix)).OrderBy(x => x.Code).ToListAsync();

            var count = 1;
            string codeTemplate = $"{prefix}{count}";
            while (true)
            {
                codeTemplate = $"{prefix}{count}";
                if (!matchedCodes.Any(x => x.Code.Equals(codeTemplate)))
                {
                    break;
                }
                count++;
            }

            var accountType = await typeObj.SearchQuery(x => x.Type.Equals("liquidity") && x.Name.ToLower().Contains(type)).FirstOrDefaultAsync();


            var account = new AccountAccount
            {
                Name = name,
                Code = codeTemplate,
                UserTypeId = accountType.Id,
                CompanyId = CompanyId,
                InternalType = accountType.Type
            };


            return account;

        }

        private async Task<string> GenerateNextCodeBankCashAsync(string type)
        {
            var prefix = type == "bank" ? "BNK" : "CSH";
            var matchedCodes = await SearchQuery(x => x.Code.Contains(prefix)).OrderBy(x => x.Code).ToListAsync();

            var count = 1;
            string codeTemplate = $"{prefix}{count}";
            while (matchedCodes.Count > 0)
            {
                codeTemplate = $"{prefix}{count}";
                if (!matchedCodes.Any(x => x.Code.Equals(codeTemplate)))
                {
                    break;
                }
                count++;
            }
            return codeTemplate;
        }

        private async Task<IRSequence> _CreateSequence(AccountJournal journal, bool refund = false)
        {
            var seqObj = GetService<IIRSequenceService>();
            var prefix = _GetSequencePrefix(journal.Code, refund: refund);
            var seq = new IRSequence
            {
                Name = journal.Name,
                Prefix = prefix,
                Padding = 4,
                NumberIncrement = 1,
                NumberNext = 1,
                CompanyId = journal.CompanyId,
            };

            return await seqObj.CreateAsync(seq);
        }

        public string _GetSequencePrefix(string code, bool refund = false)
        {
            var prefix = code.ToUpper();
            if (refund)
                prefix = "R" + prefix;
            return prefix + "/{yyyy}/";
        }

        public async Task<AccountJournal> GetJournalWithDebitCreditAccount(Guid journalId)
        {
            return await SearchQuery(x => x.Id == journalId)
                .Include(x => x.DefaultCreditAccount)
                .Include(x => x.DefaultDebitAccount)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AccountJournalSimple>> GetAutocomplete(AccountJournalFilter val)
        {
            ISpecification<AccountJournal> spec = new InitialSpecification<AccountJournal>(x => x.Active);

            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<AccountJournal>(x => x.Name.Contains(val.Search)));

            if (val.CompanyId.HasValue)
                spec = spec.And(new InitialSpecification<AccountJournal>(x => x.CompanyId == val.CompanyId));

            if (!string.IsNullOrEmpty(val.Type))
            {
                var types = val.Type.Split(",");
                spec = spec.And(new InitialSpecification<AccountJournal>(x => types.Contains(x.Type)));
            }

            var query = SearchQuery(domain: spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Code));
            var items = await query.Skip(0).Take(20).Select(x => new AccountJournalSimple()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return items;
        }

        public async Task<PagedResult2<AccountJournalBasic>> GetBankCashJournals(AccountJournalFilter val)
        {
            var types = val.Type.Split(',');
            var query = SearchQuery(x => x.Type == "bank" && x.Active);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.BankAccount.Bank.BIC.Contains(val.Search));
            if (types.Any())
                query = query.Where(x => types.Contains(x.Type));

            var res = await query
                .Include(x => x.BankAccount)
                .ThenInclude(x => x.Bank)
                .OrderBy(x => x.Code)
                .ToListAsync();
            var items = _mapper.Map<List<AccountJournalBasic>>(res);
            var totalItems = query.Count();

            return new PagedResult2<AccountJournalBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public override ISpecification<AccountJournal> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "account.journal_comp_rule":
                    return new InitialSpecification<AccountJournal>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }
    }
}
