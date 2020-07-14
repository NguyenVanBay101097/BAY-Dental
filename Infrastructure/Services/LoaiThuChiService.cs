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
    public class LoaiThuChiService : BaseService<LoaiThuChi>, ILoaiThuChiService
    {
        private readonly IMapper _mapper;

        public LoaiThuChiService(IAsyncRepository<LoaiThuChi> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<LoaiThuChiBasic>> GetThuChiPagedResultAsync(LoaiThuChiPaged val)
        {
            ISpecification<LoaiThuChi> spec = new InitialSpecification<LoaiThuChi>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<LoaiThuChi>(x => x.Name.Contains(val.Search)));

            //string name = "";
            //string reference_account_type = "";
            //if (val.Type == "thu")
            //{
            //    reference_account_type = "account.data_account_type_thu";
            //    name = "Thu";
            //}
            //else if (val.Type == "chi")
            //{
            //    reference_account_type = "account.data_account_type_chi";
            //    name = "Chi";
            //}
            //else
            //    throw new Exception("");

            //var account_type = await GetAccountTypeThuChi(reference_account_type, name);

            spec = spec.And(new InitialSpecification<LoaiThuChi>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name));

            var items = await query.Select(x => new LoaiThuChiBasic
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Note = x.Note
            }).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LoaiThuChiBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
     

        public async Task<LoaiThuChiSave> DefaultGet(AccountAccountDefault val)
        {
            
            var res = new LoaiThuChiSave();
            var objCompany = GetService<ICompanyService>();
            res.CompanyId = CompanyId;
            var company = await objCompany.GetByIdAsync(CompanyId);
            res.Company = _mapper.Map<CompanySimple>(company);
           

            return res;
        }

        public async Task<LoaiThuChi> GetByIdThuChi(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.Account).FirstOrDefaultAsync();
        }

        //create loại thu chi
        public async Task<AccountAccount> CreateAccountAccount(LoaiThuChiSave val)
        {
            
            var loaithuchi = _mapper.Map<LoaiThuChi>(val);

            if(loaithuchi.AccountId == null)
            {
                var account = await CreateOrUpdateAccount(loaithuchi);
                loaithuchi.AccountId = account.Id;
                loaithuchi.Account = account;
            }



            return await CreateAsync(loaithuchi);
        }

        public async Task<AccountAccount> CreateOrUpdateAccount(LoaiThuChi val)
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
                reference_account_type = "account.data_account_type_chi";
                name = "Chi";
            }
            var accountObj = GetService<IAccountAccountService>();

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
    }
}
