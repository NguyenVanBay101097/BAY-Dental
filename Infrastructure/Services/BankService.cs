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
    public class BankService : BaseService<Bank>, IBankService
    {
        private readonly IMapper _mapper;
        public BankService(IAsyncRepository<Bank> repository, IHttpContextAccessor httpContextAccessor) 
            : base(repository, httpContextAccessor)
        {
        }

        public async Task<PagedResult2<BankBasic>> GetPagedResultAsync(BankPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);

            var items = await query.OrderByDescending(x => x.DateCreated).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<BankBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<BankBasic>>(items)
            };
        }

        public async Task<IEnumerable<Bank>> GetAutocompleteAsync(int limit , int offset , string search)
        {
            var query = SearchQuery();
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search));

            if (limit > 0)
                query = query.Skip(offset).Take(limit);

            var items = await query.OrderByDescending(x => x.DateCreated).ToListAsync();          

            return items;
        }
    }
}
