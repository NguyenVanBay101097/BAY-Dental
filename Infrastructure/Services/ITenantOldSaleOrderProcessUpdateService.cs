using ApplicationCore.Entities;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public interface ITenantOldSaleOrderProcessUpdateService : IAdminBaseService<TenantOldSaleOrderProcessUpdate>
    {
        Task ProcessUpdate(IEnumerable<Guid> ids);
        Task<PagedResult2<TenantOldSaleOrderProcessUpdateListVM>> GetPagedResultAsync(TenantOldSaleOrderProcessUpdateFilterPagedVM val);
    }
}
