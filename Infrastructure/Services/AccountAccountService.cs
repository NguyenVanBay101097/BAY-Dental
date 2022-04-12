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
    public class AccountAccountService : BaseService<AccountAccount>, IAccountAccountService
    {
        private readonly IMapper _mapper;
        public AccountAccountService(IAsyncRepository<AccountAccount> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<AccountAccount> GetAccountIncomeCurrentCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.Code == "5111" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccountPayableCurrentCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.InternalType == "payable" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccountReceivableCurrentCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.InternalType == "receivable" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccount334CurrentCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.Code == "334" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccount3335CurrentCompany()
        {
            var companyId = CompanyId;
            var account = await SearchQuery(x => x.Code == "3335" && x.CompanyId == companyId).FirstOrDefaultAsync();
            if (account == null)
            {
                var modelDataObj = GetService<IIRModelDataService>();
                var accountType = await modelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
                account = new AccountAccount()
                {
                    Name = "Thuế thu nhập cá nhân",
                    Code = "3335",
                    UserTypeId = accountType.Id,
                    CompanyId = CompanyId,
                    InternalType = accountType.Type
                };

                await CreateAsync(account);
            }

            return account;
        }

        public async Task<AccountAccount> GetAccount3383CurrentCompany()
        {
            var companyId = CompanyId;
            var account = await SearchQuery(x => x.Code == "3383" && x.CompanyId == companyId).FirstOrDefaultAsync();
            if (account == null)
            {
                var modelDataObj = GetService<IIRModelDataService>();
                var accountType = await modelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
                account = new AccountAccount()
                {
                    Name = "Bảo hiểm xã hội",
                    Code = "3383",
                    UserTypeId = accountType.Id,
                    CompanyId = CompanyId,
                    InternalType = accountType.Type
                };

                await CreateAsync(account);
            }

            return account;
        }

        public async Task<AccountAccount> GetAccountAdvanceCurrentCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.Code == "KHTU" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccountCustomerDebtCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.Code == "CNKH" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccountCommissionAgentCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.Code == "HHNGT" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }

        public async Task<AccountAccount> GetAccountInsuranceDebtCompany()
        {
            var companyId = CompanyId;
            return await SearchQuery(x => x.Code == "CNBH" && x.CompanyId == companyId).FirstOrDefaultAsync();
        }


        public override ISpecification<AccountAccount> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "account.invoice_comp_rule":
                    return new InitialSpecification<AccountAccount>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }

        public async Task<string> SearchNewAccountCode(Guid companyId, int digits, string prefix)
        {
            foreach (var num in Enumerable.Range(1, 10000))
            {
                var newCode = prefix.PadLeft(digits - 1, '0') + num;
                var rec = await SearchQuery(x => x.Code == newCode && x.CompanyId == companyId).FirstOrDefaultAsync();
                if (rec == null)
                    return newCode;
            }

            throw new Exception("Cannot generate an unused account code.");
        }

        public async Task<IEnumerable<AccountAccount>> GetAutoCompleteAsync(AccountAccountPaged val)
        {
            var query = SearchQuery(x => x.Active);

            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Code.Contains(val.Search));

            var items = await query.ToListAsync();

            return items;
        }
    }
}
