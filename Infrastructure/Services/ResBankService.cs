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
    public class ResBankService : BaseService<ResBank>, IResBankService
    {
        private readonly IMapper _mapper;
        public ResBankService(IAsyncRepository<ResBank> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ResBankBasic>> GetPagedResultAsync(ResBankPaged paged)
        {
            var query = SearchQuery();

            if(!string.IsNullOrEmpty(paged.Search))
                query = query.Where(x=>x.Name.Contains(paged.Search) || x.BIC.Contains(paged.Search));

            var items = await query.OrderBy(x => x.Name).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<ResBankBasic>(totalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResBankBasic>>(items)
            };
        }
    }
}
