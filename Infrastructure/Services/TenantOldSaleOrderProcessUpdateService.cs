using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TenantOldSaleOrderProcessUpdateService : AdminBaseService<TenantOldSaleOrderProcessUpdate>, ITenantOldSaleOrderProcessUpdateService
    {
        public TenantOldSaleOrderProcessUpdateService(IAsyncRepository<TenantOldSaleOrderProcessUpdate> repository, IHttpContextAccessor httpContextAccessor)
            : base(repository, httpContextAccessor)
        {
        }

        public async Task ProcessUpdate(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Tenant).ToListAsync();
        }

        public async Task<PagedResult2<TenantOldSaleOrderProcessUpdateListVM>> GetPagedResultAsync(TenantOldSaleOrderProcessUpdateFilterPagedVM val)
        {
            var query = SearchQuery(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Tenant.Hostname.Contains(val.Search));

            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)
                .Select(x => new TenantOldSaleOrderProcessUpdateListVM { 
                    Id = x.Id,
                    State = x.State,
                    TenantHostName = x.Tenant.Hostname
                })
                .ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<TenantOldSaleOrderProcessUpdateListVM>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
