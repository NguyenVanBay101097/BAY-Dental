using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
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
    public class UoMCategoryService : BaseService<UoMCategory>, IUoMCategoryService
    {
        private readonly IMapper _mapper;
        public UoMCategoryService(IAsyncRepository<UoMCategory> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<UoMCategoryBasic>> GetAutocompleteAsync(UoMCategoryPaged val)
        {
            var query = GetQueryPaged(val);

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            return _mapper.Map<IEnumerable<UoMCategoryBasic>>(items);
        }

        private IQueryable<UoMCategory> GetQueryPaged(UoMCategoryPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            query = query.OrderBy(s => s.DateCreated);
            return query;
        }

        public async Task<PagedResult2<UoMCategoryBasic>> GetPagedResultAsync(UoMCategoryPaged val)
        {
            ISpecification<UoMCategory> spec = new InitialSpecification<UoMCategory>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
            {
                spec = spec.And(new InitialSpecification<UoMCategory>(x => x.Name.Contains(val.Search)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<UoMCategoryBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<UoMCategoryBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
