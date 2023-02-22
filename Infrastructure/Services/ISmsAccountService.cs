using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ISmsAccountService : IBaseService<SmsAccount>
    {
        Task<SmsAccountBasic> GetDefault();
        Task<PagedResult2<SmsAccountBasic>> GetPaged(SmsAccountPaged val);
        Task<IEnumerable<SmsSupplierBasic>> SmsSupplierAutocomplete(string Search);
    }
}
