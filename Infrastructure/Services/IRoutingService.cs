using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface IRoutingService : IBaseService<Routing>
    {
        Task<PagedResult2<Routing>> GetPagedResultAsync(RoutingPaged val);
        Task<Routing> GetRoutingForDisplayAsync(Guid id);
        Task<IEnumerable<RoutingSimple>> GetAutocompleteSimpleAsync(RoutingPaged val);
    }
}
