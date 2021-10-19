using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IBankService : IBaseService<Bank>
    {
        Task<PagedResult2<BankBasic>> GetPagedResultAsync(BankPaged val);

        Task<IEnumerable<Bank>> GetAutocompleteAsync(int limit, int offset, string search);
    }
}
