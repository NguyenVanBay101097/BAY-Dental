using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
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
    public class UoMService : BaseService<UoM>, IUoMService
    {

        public UoMService(IAsyncRepository<UoM> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<UoM> DefaultUOM()
        {
            var res = await SearchQuery().FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<UoM>> GetPagedResultAsync(UoMPaged val)
        {
            ISpecification<UoM> spec = new InitialSpecification<UoM>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
            {
                spec = spec.And(new InitialSpecification<UoM>(x => x.Name.Contains(val.Search)));
            }

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated), limit: val.Limit, offSet: val.Offset);
            var items = await query.ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<UoM>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }
    }
}
