using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IResBankService : IBaseService<ResBank>
    {
        Task<PagedResult2<ResBankBasic>> GetPagedResultAsync(ResBankPaged paged);
        Task<IEnumerable<ResBankSimple>> AutocompleteAsync(ResPartnerBankPaged val);
    }
}
