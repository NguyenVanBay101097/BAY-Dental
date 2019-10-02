using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountAccountService: IBaseService<AccountAccount>
    {
        Task<AccountAccount> GetAccountReceivableCurrentCompany();
        Task<AccountAccount> GetAccountPayableCurrentCompany();
    }
}
