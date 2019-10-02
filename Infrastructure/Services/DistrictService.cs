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
    public class DistrictService : BaseService<District>, IDistrictService
    {
        public DistrictService(IAsyncRepository<District> repository, IHttpContextAccessor httpContextAccessor)
           : base(repository, httpContextAccessor)
        {
        }

        public async Task<District> GetDistrictForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Province)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<District>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "")
        {
            Expression<Func<District, object>> sort = null;
            switch (orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new DistrictFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new DistrictFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await base.ListAsync(filterPaginatedSpecification);
            var totalItems = await base.CountAsync(filterSpecification);

            return new PagedResult<District>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<District> CreateDistrictAsync(District district)
        {
            return await CreateAsync(district);
        }

        public async Task UnlinkDistrictAsync(District district)
        {
            await DeleteAsync(district);
        }

        public async Task<District> GetDistrictByIdAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task UpdateDistrictAsync(District district)
        {
            await UpdateAsync(district);
        }
    }
}
