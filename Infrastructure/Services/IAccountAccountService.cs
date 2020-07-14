using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IAccountAccountService: IBaseService<AccountAccount>
    {
        Task<AccountAccount> GetAccountReceivableCurrentCompany();
        Task<AccountAccount> GetAccountPayableCurrentCompany();
        Task<PagedResult2<AccountAccountBasic>> GetPagedResultAsync(AccountAccountPaged val);

        Task<AccountAccountSave> DefaultGet(AccountAccountDefault val);

        Task<AccountAccount> GetById(Guid id);

        Task<AccountAccount> CreateAccountAccount(AccountAccountSave val);
        Task UpdateAccountAccount(Guid id, AccountAccountSave val);
    }
}
