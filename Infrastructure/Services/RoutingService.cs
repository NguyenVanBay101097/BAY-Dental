using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
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
    public class RoutingService : BaseService<Routing>, IRoutingService
    {
        private readonly IMapper _mapper;
        public RoutingService(IAsyncRepository<Routing> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<Routing>> GetPagedResultAsync(RoutingPaged val)
        {
            var query = SearchQuery(domain: x => string.IsNullOrEmpty(val.Search) || x.Name.Contains(val.Search) ||
            x.Product.Name.Contains(val.Search) || x.Product.NameNoSign.Contains(val.Search));
            if (val.ProductId.HasValue)
                query = query.Where(x => x.ProductId == val.ProductId);
            var items = await query.OrderBy(x => x.Name).Skip(val.Offset).Take(val.Limit).Include(x => x.Product).ToListAsync();
            var totalItems = await query.CountAsync();

            return new PagedResult2<Routing>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<IEnumerable<RoutingSimple>> GetAutocompleteSimpleAsync(RoutingPaged val)
        {
            var query = GetQueryPaged(val);

            var items = await query.Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoutingSimple>>(items);
        }

        private IQueryable<Routing> GetQueryPaged(RoutingPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Product.Name.Contains(val.Search) ||
                x.Product.NameNoSign.Contains(val.Search));

            query = query.OrderBy(s => s.Name);
            return query;
        }

        public async Task<Routing> GetRoutingForDisplayAsync(Guid id)
        {
            return await SearchQuery(x => x.Id == id)
                .Include(x => x.Product)
                .Include(x => x.Lines)
                .Include("Lines.Product")
                .FirstOrDefaultAsync();
        }
    }
}
