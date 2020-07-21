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
    }
}
