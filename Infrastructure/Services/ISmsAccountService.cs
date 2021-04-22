using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsAccountService : IBaseService<SmsAccount>
    {
        Task CreateAsync(SmsAccountSave val);
        Task UpdateAsync(Guid id, SmsAccountSave val);
        Task<SmsAccountBasic> GetDefault();
    }
}
