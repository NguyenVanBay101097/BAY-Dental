using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountAccountTypeService : BaseService<AccountAccountType>, IAccountAccountTypeService
    {

        public AccountAccountTypeService(IAsyncRepository<AccountAccountType> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<AccountAccountType> GetDefaultAccountTypeThu()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var accountType = await modelDataObj.GetRef<AccountAccountType>("account.data_account_type_thu");
            if (accountType == null)
            {
                accountType = new AccountAccountType
                {
                    Name = "Thu",
                    Type = "other"
                };

                await CreateAsync(accountType);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "data_account_type_thu",
                    Module = "account",
                    Model = "account.account.type",
                    ResId = accountType.Id.ToString()
                });
            }

            return accountType;
        }

        public async Task<AccountAccountType> GetDefaultAccountTypeChi()
        {
            var modelDataObj = GetService<IIRModelDataService>();
            var accountType = await modelDataObj.GetRef<AccountAccountType>("account.data_account_type_chi");
            if (accountType == null)
            {
                accountType = new AccountAccountType
                {
                    Name = "Chi",
                    Type = "other"
                };

                await CreateAsync(accountType);

                await modelDataObj.CreateAsync(new IRModelData
                {
                    Name = "data_account_type_chi",
                    Module = "account",
                    Model = "account.account.type",
                    ResId = accountType.Id.ToString()
                });
            }

            return accountType;
        }
    }
}
