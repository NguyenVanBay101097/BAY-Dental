using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.TenantData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TenantService : AdminBaseService<AppTenant>, ITenantService
    {
        private readonly IMapper _mapper;
        public TenantService(IAsyncRepository<AppTenant> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<TenantBasic>> GetPagedResultAsync(TenantPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<TenantBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<TenantBasic>>(items)
            };
        }

        private IQueryable<AppTenant> GetQueryPaged(TenantPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Phone.Contains(val.Search) ||
                x.Hostname.Contains(val.Search));

            return query;
        }
    }
}
