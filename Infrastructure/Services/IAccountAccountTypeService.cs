using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IAccountAccountTypeService: IBaseService<AccountAccountType>
    {
        Task<AccountAccountType> GetDefaultAccountTypeThu();
        Task<AccountAccountType> GetDefaultAccountTypeChi();
    }
}
