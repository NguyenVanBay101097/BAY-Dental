using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Data;
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
    public class HrJobService : BaseService<HrJob>, IHrJobService
    {
        private readonly IMapper _mapper;
        private readonly CatalogDbContext _context;
        public HrJobService(IAsyncRepository<HrJob> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, CatalogDbContext context)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<PagedResult2<HrJobBasic>> GetPagedResultAsync(HrJobPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            var totalItem = await query.CountAsync();
            query = query.OrderByDescending(x => x.DateCreated);
            var items = await _mapper.ProjectTo<HrJobBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            
            return new PagedResult2<HrJobBasic>(totalItem, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrJobBasic>>(items)
            };
        }

        public async Task<PagedResult2<HrJobBasic>> AutoComplete(HrJobPaged val)
        {
            var query = SearchQuery(x => x.CompanyId == CompanyId);
            if (!string.IsNullOrWhiteSpace(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));
            var count = await query.CountAsync();
            if (val.Limit > 0)
                query = query.Skip(val.Offset).Take(val.Limit);
            var res = await query.ToListAsync();
            return new PagedResult2<HrJobBasic>(count, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrJobBasic>>(res)
            };
        }
    }
}
