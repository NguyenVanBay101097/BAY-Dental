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

        public async Task<PagedResult2<AccountAccountBasic>> GetThuChiPagedResultAsync(AccountAccountThuChiPaged val)
        {
            ISpecification<AccountAccount> spec = new InitialSpecification<AccountAccount>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<AccountAccount>(x => x.Name.Contains(val.Search)));
            string name = "";
            string reference_account_type = "";
            if (val.Type == "thu")
            {
                reference_account_type = "account.data_account_type_thu";
                name = "Thu";
            }              
            else if (val.Type == "chi")
            {
                reference_account_type = "account.data_account_type_chi";
                name = "Chi";
            }               
            else
                throw new Exception("");

            var account_type = await GetAccountTypeThuChi(reference_account_type , name);

            spec = spec.And(new InitialSpecification<AccountAccount>(x => x.UserTypeId == account_type.Id));

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
            string name = "";
            string reference_account_type = "";
            if (val.Type == "thu")
            {
                reference_account_type = "account.data_account_type_thu";
                name = "Thu";
            }
            else if (val.Type == "chi")
            {
                reference_account_type = "account.data_account_type_thu";
                name = "Chi";
            }
            var res = new AccountAccountSave();
            var objCompany = GetService<ICompanyService>();
            res.CompanyId = CompanyId;
            var company = await objCompany.GetByIdAsync(CompanyId);
            res.Company = _mapper.Map<CompanySimple>(company);
            var usertype = await GetAccountTypeThuChi(reference_account_type, name);
            if (usertype != null)
            {
                res.UserTypeId = usertype.Id;

            }
            res.Reconcile = false;

            return res;
        }

        public async Task<AccountAccount> GetByIdThuChi(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.UserType).FirstOrDefaultAsync();
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
            accountAccount.InternalType = val.UserType.Type;

            await UpdateAsync(accountAccount);
        }


        public async Task<AccountAccountType> GetAccountTypeThuChi(string reference, string name)
        {
            //reference: account.data_account_type_thu
            var accountTypeObj = GetService<IAccountAccountTypeService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var accountType = await modelDataObj.GetRef<AccountAccountType>(reference);
            if (accountType == null)
            {
                //tao account type
                accountType = new AccountAccountType()
                {
                    Name = name,
                    Type = "other",                    
                };
                await accountTypeObj.CreateAsync(accountType);
                //them vao ir model data
                var tmp = reference.Split(".");
                var modeldata = new IRModelData
                {
                    Model = "account.account.type",
                    Module = tmp[0],
                    Name = tmp[1],
                    ResId = accountType.Id.ToString()
                };
                await modelDataObj.CreateAsync(modeldata);
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
