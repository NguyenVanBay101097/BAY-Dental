using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class WardService : BaseService<Ward>, IWardService
    {
        public WardService(IAsyncRepository<Ward> repository, IHttpContextAccessor httpContextAccessor)
           : base(repository, httpContextAccessor)
        {
        }

        public async Task<Ward> GetWardForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.District)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Ward>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "")
        {
            Expression<Func<Ward, object>> sort = null;
            switch (orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new WardFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new WardFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await base.ListAsync(filterPaginatedSpecification);
            var totalItems = await base.CountAsync(filterSpecification);

            return new PagedResult<Ward>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<Ward> CreateWardAsync(Ward ward)
        {
            return await CreateAsync(ward);
        }

        public async Task UnlinkWardAsync(Ward ward)
        {
            await DeleteAsync(ward);
        }

        public async Task<Ward> GetWardByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task UpdateWardAsync(Ward ward)
        {
            await UpdateAsync(ward);
        }
    }
}
