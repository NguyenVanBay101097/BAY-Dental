using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
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
    public class HistoryService : BaseService<History>, IHistoryService
    {

        private readonly IMapper _mapper;
        public HistoryService(IAsyncRepository<History> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            :base (repository,httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<HistorySimple>> GetAutocompleteAsync(HistoryPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            return _mapper.Map<IEnumerable<HistorySimple>>(items);
        }

        public async Task<PagedResult2<HistorySimple>> GetPagedResultAsync(HistoryPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<HistorySimple>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HistorySimple>>(items)
            };
        }

        public async Task<IEnumerable<HistorySimple>> GetResultNotLimitAsync(HistoryPaged val)
        {
            var query = GetQueryPaged(val);

            return _mapper.Map<IEnumerable<HistorySimple>>(query);
        }

        private IQueryable<History> GetQueryPaged(HistoryPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            query = query.OrderBy(x => x.Name);
            return query;
        }

        public async Task<bool> CheckDuplicate(Guid? id, HistorySimple val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Name.Trim()))
                query = query.Where(x => x.Name.Trim().ToLower().Equals(val.Name.Trim().ToLower()));
            if (id != Guid.Empty)
                query = query.Where(x=>x.Id!= id);

            var count = await query.CountAsync();
            if(count > 0)
            {
                return true;
            } else
            {
                return false;
            }
            
        }
    }
}
