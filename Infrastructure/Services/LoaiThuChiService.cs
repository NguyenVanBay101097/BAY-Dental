using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
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
                spec = spec.And(new InitialSpecification<LoaiThuChi>(x => x.Name.Contains(val.Search) || x.Code.Contains(val.Search)));
            if (val.CompanyId.HasValue)
                spec = spec.And(new InitialSpecification<LoaiThuChi>(x => x.CompanyId == val.CompanyId));

            spec = spec.And(new InitialSpecification<LoaiThuChi>(x => x.Type == val.Type));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderBy(s => s.Name));

            var items = await query.Select(x => new LoaiThuChiBasic
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Note = x.Note,
                IsAccounting = x.IsAccounting
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
            res.IsAccounting = false;
            return res;
        }

        public async Task<LoaiThuChi> GetByIdThuChi(Guid id)
        {
            return await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.Account).FirstOrDefaultAsync();
        }

        //create loại thu chi
        public async Task<LoaiThuChi> CreateLoaiThuChi(LoaiThuChiSave val)
        {
            var self = _mapper.Map<LoaiThuChi>(val);
            string reference_account_type = "";

            if (self.Type == "thu")
            {
                if (self.IsAccounting)
                    reference_account_type = "account.data_account_type_revenue";
                else
                    reference_account_type = "account.data_account_type_thu";
            }
            else if (self.Type == "chi")
            {
                if (self.IsAccounting)
                    reference_account_type = "account.data_account_type_expenses";
                else
                    reference_account_type = "account.data_account_type_chi";
            }

            var usertype = await GetAccountTypeThuChi(reference_account_type);
            var account = new AccountAccount
            {
                Name = self.Name,
                Code = self.Code,
                Note = self.Note,
                CompanyId = self.CompanyId ?? CompanyId,
                InternalType = usertype.Type,
                UserTypeId = usertype.Id,
            };

            var accountObj = GetService<IAccountAccountService>();
            await accountObj.CreateAsync(account);
            self.AccountId = account.Id;

            return await CreateAsync(self);
        }

        //update loại thu chi
        public async Task UpdateLoaiThuChi(Guid id, LoaiThuChiSave val)
        {
            var self = await SearchQuery(x => x.Id == id).Include(x => x.Company).Include(x => x.Account).FirstOrDefaultAsync();
            if (self == null)
                throw new Exception("Loại không tồn tại");

            self = _mapper.Map(val, self);

            var account = self.Account;
            if (account != null)
            {
                var accountObj = GetService<IAccountAccountService>();
                account.Name = val.Name;
                account.Code = val.Code;
                account.Note = val.Note;

                string reference_account_type = "";
                if (self.Type == "thu")
                {
                    if (self.IsAccounting)
                        reference_account_type = "account.data_account_type_revenue";
                    else
                        reference_account_type = "account.data_account_type_thu";
                }
                else if (self.Type == "chi")
                {
                    if (self.IsAccounting)
                        reference_account_type = "account.data_account_type_expenses";
                    else
                        reference_account_type = "account.data_account_type_chi";
                }

                var usertype = await GetAccountTypeThuChi(reference_account_type);
                account.UserTypeId = usertype.Id;

                await accountObj.UpdateAsync(account);
            }

            await UpdateAsync(self);
        }

        public async Task RemoveLoaiThuChi(Guid id)
        {
            var accountObj = GetService<IAccountAccountService>();
            var loaithuchi = await SearchQuery(x => x.Id == id)
                .Include(x => x.Account)
                .FirstOrDefaultAsync();

            if (loaithuchi == null)
                throw new Exception("Loại không tồn tại");

            var account = loaithuchi.Account;
            await DeleteAsync(loaithuchi);

            if (account != null)
                await accountObj.DeleteAsync(account);
        }

        public async Task<AccountAccountType> GetAccountTypeThuChi(string reference)
        {
            //reference: account.data_account_type_thu
            var accountTypeObj = GetService<IAccountAccountTypeService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var accountType = await modelDataObj.GetRef<AccountAccountType>(reference);
            if (accountType == null)
            {
                string name = "";
                if (reference == "account.data_account_type_thu")
                    name = "Thu";
                else if (reference == "account.data_account_type_chi")
                    name = "Chi";
                else if (reference == "account.data_account_type_revenue")
                    name = "Income";
                else if (reference == "account.data_account_type_expenses")
                    name = "Expenses";

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

        public async Task InsertModelsIfNotExists()
        {
            var modelObj = GetService<IIRModelService>();
            var modelDataObj = GetService<IIRModelDataService>();
            var model = await modelDataObj.GetRef<IRModel>("account.model_loai_thu_chi");
            if (model == null)
            {
                model = new IRModel
                {
                    Name = "Loại thu chi",
                    Model = "LoaiThuChi",
                };

                modelObj.Sudo = true;
                await modelObj.CreateAsync(model);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "model_loai_thu_chi",
                    Module = "account",
                    Model = "ir.model",
                    ResId = model.Id.ToString()
                });
            }
        }

        public override ISpecification<LoaiThuChi> RuleDomainGet(IRRule rule)
        {
            //var userObj = GetService<IUserService>();
            //var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "account.loai_thu_chi_comp_rule":
                    return new InitialSpecification<LoaiThuChi>(x => !x.CompanyId.HasValue || x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }
}
