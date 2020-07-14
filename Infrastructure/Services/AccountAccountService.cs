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

        public async Task<PagedResult2<AccountAccountBasic>> GetPagedResultAsync(AccountAccountPaged val)
        {
            ISpecification<AccountAccount> spec = new InitialSpecification<AccountAccount>(x => true);
            if (!string.IsNullOrEmpty(val.Type))
                spec = spec.And(new InitialSpecification<AccountAccount>(x => x.UserType.Type == val.Type));
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<AccountAccount>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name));

            var items = await query.Select(x => new AccountAccountBasic
            {
                Id = x.Id,
                Name = x.Name,
                CompanyName = x.Company.Name,
                Code = x.Code,
                Note = x.Note
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<AccountAccountBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
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

        public async Task<AccountAccountSave> DefaultGet(AccountAccountDefault val)
        {

            var res = new AccountAccountSave();
            var objCompany = GetService<ICompanyService>();
            res.CompanyId = CompanyId;
            var company = await objCompany.GetByIdAsync(CompanyId);
            res.Company = _mapper.Map<CompanySimple>(company);
            var usertype = await CheckAccountType(val);
            if (usertype != null)
            {
                res.UserTypeId = usertype.Id;
                res.UserType = usertype;

            }
            res.Active = true;
            res.Reconcile = false;

            return res;
        }

        public async Task<AccountAccount> GetById (Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Company)
                .Include(x => x.UserType)
                .FirstOrDefaultAsync();
        }

        //create loại thu chi
        public async Task<AccountAccount> CreateAccountAccount(AccountAccountSave val)
        {
            var accountaccount = _mapper.Map<AccountAccount>(val);
            accountaccount.InternalType = val.UserType.Type;

            return await CreateAsync(accountaccount);
        }

        //update loại thu chi
        public async Task UpdateAccountAccount(Guid id, AccountAccountSave val)
        {
            var accountAccount = await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.UserType).FirstOrDefaultAsync();
            if (accountAccount == null)
                throw new Exception("Loại không tồn tại");

            accountAccount = _mapper.Map(val, accountAccount);

            accountAccount.InternalType = accountAccount.UserType.Type;

            await UpdateAsync(accountAccount);
        }

        public async Task<AccountAccountType> CheckAccountType(AccountAccountDefault val)
        {
            var accountType = new AccountAccountType();

            var accountTypeService = GetService<IAccountAccountTypeService>();
            var objModel = GetService<IIRModelDataService>();
            if (val.Type == "paymenttype")
            {
                accountType = await objModel.GetRef<AccountAccountType>("accounttype.payment_type");
                if (accountType == null)
                {
                    accountType = await GetOrCreateAccountType(val);
                    var modeldata = new IRModelData
                    {
                        Model = "account.type.paymment",
                        Module = "accounttype",
                        Name = "paymment_type",
                        ResId = accountType.Id.ToString()
                    };
                    await objModel.CreateAsync(modeldata);
                }
            }
            else
            {
                accountType = await objModel.GetRef<AccountAccountType>("accounttype.receipt_type");
                if (accountType == null)
                {
                    accountType = await GetOrCreateAccountType(val);
                    var modeldata = new IRModelData
                    {
                        Model = "account.type.receipt",
                        Module = "accounttype",
                        Name = "receipt_type",
                        ResId = accountType.Id.ToString()
                    };
                    await objModel.CreateAsync(modeldata);
                }
            }


            return accountType;
        }

        private async Task<AccountAccountType> GetOrCreateAccountType(AccountAccountDefault val)
        {
            var accountTypeObj = GetService<IAccountAccountTypeService>();
            var accountType = new AccountAccountType();
            if (val.Type == "paymenttype")
            {
                accountType = new AccountAccountType()
                {
                    Name = "Payment Type",
                    Type = "paymenttype",
                    IncludeInitialBalance = false,
                };
                await accountTypeObj.CreateAsync(accountType);
            }
            else
            {
                accountType = new AccountAccountType()
                {
                    Name = "Receipt Type",
                    Type = "receipttype",
                    IncludeInitialBalance = false,
                };
                await accountTypeObj.CreateAsync(accountType);
            }


            return accountType;
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
