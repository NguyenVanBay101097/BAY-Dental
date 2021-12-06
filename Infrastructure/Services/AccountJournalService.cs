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
            return await SearchQuery(x => x.Type == type && x.CompanyId == companyId).Include(x => x.DefaultDebitAccount).Include(x => x.DefaultCreditAccount).FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<AccountJournal>> CreateAsync(IEnumerable<AccountJournal> self)
        {
            foreach (var journal in self)
            {
                if (journal.SequenceId != Guid.Empty)
                    continue;

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

        public async Task<IEnumerable<AccountJournal>> CreateJournals(IEnumerable<AccountJournalSave> vals)
        {
            var accountObj = GetService<IAccountAccountService>();
            var journals = new List<AccountJournal>();
            foreach (var val in vals)
            {
                var journal = new AccountJournal();

                if (val.Type == "bank" && !journal.BankAccountId.HasValue && !string.IsNullOrEmpty(val.AccountNumber))
                {
                    var partnerBank = await SetPartnerBankAsync(val.AccountNumber, val.BankId);
                    partnerBank.AccountHolderName = val.AccountHolderName;
                    partnerBank.Branch = val.BankBranch;
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
                journals.Add(journal);
            }

            await base.CreateAsync(journals);
            return journals;
        }

        public async Task<AccountJournal> CreateBankJournal(AccountJournalCreateBankJournalVM val)
        {
            var bankObj = GetService<IResBankService>();
            var bank = await bankObj.GetByIdAsync(val.BankId);

            var journal = new AccountJournal()
            {
                Active = val.Active,
                Name = $"{val.AccountNumber} - {bank.BIC}",
                Type = "bank",
                CompanyId = CompanyId
            };

            var resPnBankObj = GetService<IResPartnerBankService>();
            var companyObj = GetService<ICompanyService>();

            var company = await companyObj.GetByIdAsync(CompanyId);
            var partnerBank = new ResPartnerBank
            {
                AccountNumber = val.AccountNumber,
                BankId = val.BankId,
                PartnerId = company.PartnerId,
                AccountHolderName = val.AccountHolderName,
                Branch = val.BankBranch
            };

            await resPnBankObj.CreateAsync(partnerBank);

            journal.BankAccountId = partnerBank.Id;

            journal.Code = await GenerateNextCodeBankCashAsync("bank");
            if (string.IsNullOrEmpty(journal.Code))
                throw new Exception("Không thể phát sinh code");

            var account = await PrepareAccountLiquidityAsync("bank", journal.Name);

            var accountObj = GetService<IAccountAccountService>();
            await accountObj.CreateAsync(account);

            journal.DefaultCreditAccountId = account.Id;
            journal.DefaultDebitAccountId = account.Id;

            //tạo sequence
            var seq = await _CreateSequence(journal);
            journal.SequenceId = seq.Id;

            await CreateAsync(journal);
            return journal;
        }

        public async Task UpdateBankJournal(AccountJournalUpdateBankJournalVM val)
        {
            var journal = await SearchQuery(x => x.Id == val.Id)
                .Include(x => x.BankAccount)
                .FirstOrDefaultAsync();

            var bankAcc = journal.BankAccount;
            if (bankAcc != null)
            {
                bankAcc.AccountNumber = val.AccountNumber;
                bankAcc.AccountHolderName = val.AccountHolderName;
                bankAcc.Branch = val.BankBranch;
                bankAcc.BankId = val.BankId;
            }
            else
            {
                var resPnBankObj = GetService<IResPartnerBankService>();
                var companyObj = GetService<ICompanyService>();

                var company = await companyObj.GetByIdAsync(CompanyId);
                var partnerBank = new ResPartnerBank
                {
                    AccountNumber = val.AccountNumber,
                    BankId = val.BankId,
                    PartnerId = company.PartnerId,
                    AccountHolderName = val.AccountHolderName,
                    Branch = val.BankBranch
                };

                await resPnBankObj.CreateAsync(partnerBank);

                journal.BankAccountId = partnerBank.Id;
            }

            var bankObj = GetService<IResBankService>();
            var bank = await bankObj.GetByIdAsync(val.BankId);

            journal.Name = $"{val.AccountNumber} - {bank.BIC}";
            journal.Active = val.Active;

            await UpdateAsync(journal);
        }

        public async Task UpdateJournalSave(Guid id, AccountJournalSave val)
        {
            var resPnBankObj = GetService<IResPartnerBankService>();
            var accountObj = GetService<IAccountAccountService>();

            var journal = await SearchQuery(x => x.Id == id)
                .Include(x => x.BankAccount)
                //.ThenInclude(x => x.Bank)
                .FirstOrDefaultAsync();

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

            if (val.BankId.HasValue && journal.BankAccount != null && !string.IsNullOrEmpty(val.AccountNumber))
            {
                journal.BankAccount.BankId = val.BankId.Value;
                journal.BankAccount.AccountNumber = val.AccountNumber;
                journal.BankAccount.AccountHolderName = val.AccountHolderName;
                journal.BankAccount.Branch = val.BankBranch;
            }
            journal.Active = val.Active;

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
            return string.Format("{0} - {1}", accountNumber, bank.BIC);
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
            var digits = 6;
            var accountObj = GetService<IAccountAccountService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var acc = await accountObj.SearchQuery(x => x.InternalType == "liquidity" && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (acc != null)
                digits = acc.Code.Length;
            var account_code_prefix = type == "bank" ? "1112" : "1111";
            var liquidity_type = await modelDataObj.GetRef<AccountAccountType>("account.data_account_type_liquidity");

            var account = new AccountAccount
            {
                Name = name,
                Code = await accountObj.SearchNewAccountCode(CompanyId, digits, account_code_prefix),
                UserTypeId = liquidity_type.Id,
                CompanyId = CompanyId,
                InternalType = liquidity_type.Type
            };

            return account;

        }

        private async Task<string> GenerateNextCodeBankCashAsync(string type)
        {
            var journal_code_base = type == "bank" ? "BNK" : "CSH";
            var matchedCodes = await SearchQuery(x => x.Code.Contains(journal_code_base) && x.CompanyId == CompanyId).Select(x => x.Code).ToListAsync();
            foreach(var num in Enumerable.Range(1, 100))
            {
                var journalCode = journal_code_base + num;
                if (!matchedCodes.Contains(journalCode))
                    return journalCode;
            }

            return string.Empty;
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

        public async Task<IEnumerable<AccountJournal>> GetJournalWithDebitCreditAccount(IEnumerable<Guid> journalIds)
        {
            return await SearchQuery(x => journalIds.Any(z => z == x.Id))
                .Include(x => x.DefaultCreditAccount)
                .Include(x => x.DefaultDebitAccount)
                .ToListAsync();
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
            
            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            } 
            var items = await query.Select(x => new AccountJournalSimple()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
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
            //var userObj = GetService<IUserService>();
            //var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "account.journal_comp_rule":
                    return new InitialSpecification<AccountJournal>(x => x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }

        public async Task Unlink(Guid id)
        {
            var resPartnerBankObj = GetService<IResPartnerBankService>();
            var journal = await SearchQuery(x => x.Id == id)
               .Include(x => x.BankAccount)
               .FirstOrDefaultAsync();

            if (journal == null) 
                throw new Exception("Không tìm thấy phương thức thanh toán");
            await DeleteAsync(journal);
            await resPartnerBankObj.DeleteAsync(journal.BankAccount);
        }

        public async Task<IEnumerable<AccountJournalResBankSimple>> GetJournalResBankAutocomplete(AccountJournalFilter val)
        {
            ISpecification<AccountJournal> spec = new InitialSpecification<AccountJournal>(x => true);

            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<AccountJournal>(x => x.Name.Contains(val.Search)));

            if (val.CompanyId.HasValue)
                spec = spec.And(new InitialSpecification<AccountJournal>(x => x.CompanyId == val.CompanyId));

            if (val.Active.HasValue)
                spec = spec.And(new InitialSpecification<AccountJournal>(x => x.Active == val.Active));

            if (!string.IsNullOrEmpty(val.Type))
            {
                var types = val.Type.Split(",");
                spec = spec.And(new InitialSpecification<AccountJournal>(x => types.Contains(x.Type)));
            }

            var query = SearchQuery(domain: spec.AsExpression(), orderBy: x => x.OrderByDescending(z=> z.Active).ThenBy(s=> s.Code));
            var items = await query.Include(x=> x.BankAccount.Bank).Skip(0).Take(20).ToListAsync();

            return _mapper.Map<IEnumerable<AccountJournalResBankSimple>>(items);
        }
    }
}
