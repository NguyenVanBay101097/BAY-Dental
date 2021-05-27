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
        Task<AccountAccount> GetAccountIncomeCurrentCompany();
        Task<AccountAccount> GetAccount334CurrentCompany();
        Task<AccountAccount> GetAccountAdvanceCurrentCompany();

        Task<AccountAccount> GetAccountCommissionAgentCompany();
        Task<AccountAccount> GetAccountCustomerDebtCompany();
    }
}
