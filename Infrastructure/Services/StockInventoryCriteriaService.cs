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
    public class StockInventoryCriteriaService : BaseService<StockInventoryCriteria>, IStockInventoryCriteriaService
    {
        private readonly IMapper _mapper;
        public StockInventoryCriteriaService(IAsyncRepository<StockInventoryCriteria> repository, IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<StockInventoryCriteriaBasic>> GetPagedResultAsync(StockInventoryCriteriaPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Name.Contains(val.Search));
            }

            var count = await query.CountAsync();

            if (val.Limit > 0)
            {
                query = query.Skip(val.Offset).Take(val.Limit);
            }

            var items = await _mapper.ProjectTo<StockInventoryCriteriaBasic>(query).ToListAsync();


            return new PagedResult2<StockInventoryCriteriaBasic>(count, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
