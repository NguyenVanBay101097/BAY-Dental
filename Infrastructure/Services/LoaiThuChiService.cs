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

            spec = spec.And(new InitialSpecification<LoaiThuChi>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name));

            var items = await query.Select(x => new LoaiThuChiBasic
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Note = x.Note
            }).Skip(val.Offset).Take(val.Limit).ToListAsync();

            var totalItems = await query.CountAsync();
            return new PagedResult2<LoaiThuChiBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }


        public LoaiThuChiDisplay DefaultGet(LoaiThuChiDefault val)
        {
            var res = new LoaiThuChiDisplay();
            res.Type = val.Type;
            res.CompanyId = CompanyId;
            res.IsInclude = true;
            return res;
        }

        public async Task<LoaiThuChi> GetByIdThuChi(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.Account).FirstOrDefaultAsync();
        }

        //create loại thu chi
        public async Task<LoaiThuChi> CreateLoaiThuChi(LoaiThuChiSave val)
        {
            var loaithuchi = _mapper.Map<LoaiThuChi>(val);
            var account = await GenerateAccountThuChi(loaithuchi);
            loaithuchi.AccountId = account.Id;

            return await CreateAsync(loaithuchi);
        }

        public async Task<AccountAccount> GenerateAccountThuChi(LoaiThuChi self)
        {
            var accountObj = GetService<IAccountAccountService>();
            string name = "";
            string reference_account_type = "";
            if (self.Type == "thu")
            {
                reference_account_type = "account.data_account_type_thu";
                name = "Thu";
            }
            else if (self.Type == "chi")
            {
                reference_account_type = "account.data_account_type_chi";
                name = "Chi";
            }

            var usertype = await GetAccountTypeThuChi(reference_account_type, name);
            var account = new AccountAccount
            {
                Name = self.Name,
                Code = self.Code,
                Note = self.Note,
                CompanyId = self.CompanyId ?? CompanyId,
                IsExcludedProfitAndLossReport = !self.IsInclude,
                InternalType = usertype.Type,
                UserTypeId = usertype.Id,
            };
           
            await accountObj.CreateAsync(account);
            return account;
        }

        //update loại thu chi
        public async Task UpdateLoaiThuChi(Guid id, LoaiThuChiSave val)
        {
            var loaithuchi = await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.Account).FirstOrDefaultAsync();
            if (loaithuchi == null)
                throw new Exception("Loại không tồn tại");

            loaithuchi = _mapper.Map(val, loaithuchi);

            var account = loaithuchi.Account;
            if (account != null)
            {
                var accountObj = GetService<IAccountAccountService>();
                account.Name = val.Name;
                account.Code = val.Code;
                account.Note = val.Note;
                account.IsExcludedProfitAndLossReport = !val.IsInclude;
                await accountObj.UpdateAsync(account);
            }

            await UpdateAsync(loaithuchi);
        }

        public async Task RemoveLoaiThuChi(Guid id)
        {
            var accountObj = GetService<IAccountAccountService>();
            var loaithuchi = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (loaithuchi == null)
                throw new Exception("Loại không tồn tại");
            await DeleteAsync(loaithuchi);
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
