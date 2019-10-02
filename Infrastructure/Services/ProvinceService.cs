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
    public class ProvinceService : BaseService<Province>, IProvinceService
    {
        public ProvinceService(IAsyncRepository<Province> repository, IHttpContextAccessor httpContextAccessor)
           : base(repository, httpContextAccessor)
        {
        }

        public async Task<PagedResult<Province>> GetPagedResultAsync(int pageIndex = 0, int pageSize = 20, string orderBy = "name", string orderDirection = "asc", string filter = "")
        {
            Expression<Func<Province, object>> sort = null;
            switch (orderBy)
            {
                case "name":
                    sort = x => x.Name;
                    break;
                default:
                    break;
            }

            var filterSpecification = new ProvinceFilterSpecification(filter: filter, orderBy: sort, orderDirection: orderDirection);
            var filterPaginatedSpecification = new ProvinceFilterSpecification(filter: filter, skip: pageIndex * pageSize, take: pageSize, isPagingEnabled: true, orderBy: sort, orderDirection: orderDirection);
            var items = await ListAsync(filterPaginatedSpecification);
            var totalItems = await CountAsync(filterSpecification);

            return new PagedResult<Province>(totalItems, pageIndex + 1, pageSize)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<ProvinceSimple>> GetProvincesAutocomplete(string filter = "")
        {
            return await SearchQuery(domain: x => x.Name.Contains(filter), orderBy: x => x.OrderBy(s => s.Name), limit: 10).Select(x => new ProvinceSimple
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
