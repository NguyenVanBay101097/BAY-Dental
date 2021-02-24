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
    public class EmployeeAdminService : AdminBaseService<EmployeeAdmin>, IEmployeeAdminService
    {
        private readonly IMapper _mapper;
        public EmployeeAdminService(IAsyncRepository<EmployeeAdmin> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<EmployeeAdmin>> GetPagedResultAsync(EmployeeAdminPaged val)
        {
            var query = GetQueryPaged(val);
            var items = await query.OrderByDescending(x => x.DateCreated).Skip(val.Offset).Take(val.Limit)
                .ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<EmployeeAdmin>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<EmployeeAdmin>>(items)
            };
        }

        private IQueryable<EmployeeAdmin> GetQueryPaged(EmployeeAdminPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search));

            return query;
        }
    }
}
