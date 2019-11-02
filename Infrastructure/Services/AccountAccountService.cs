using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountAccountService : BaseService<AccountAccount>, IAccountAccountService
    {
        public AccountAccountService(IAsyncRepository<AccountAccount> repository, IHttpContextAccessor httpContextAccessor)
           : base(repository, httpContextAccessor)
        {
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

        public override ISpecification<AccountAccount> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.invoice_comp_rule":
                    return new InitialSpecification<AccountAccount>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
