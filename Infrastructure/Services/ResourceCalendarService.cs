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
using Umbraco.Web.Mapping;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ResourceCalendarService : BaseService<ResourceCalendar>, IResourceCalendarService
    {
        private readonly IMapper _mapper;
        public ResourceCalendarService(IMapper mapper, IAsyncRepository<ResourceCalendar> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ResourceCalendar> GetDisplayAsync(Guid id)
        {
            var res = await SearchQuery(x => x.Id == id).Include("ResourceCalendarAttendances").FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<ResourceCalendarBasic>> GetPaged(ResourceCalendarPaged paged)
        {
            ISpecification<ResourceCalendar> spec = new InitialSpecification<ResourceCalendar>(x => true);

            if (!string.IsNullOrEmpty(paged.Search))
                spec = spec.And(new InitialSpecification<ResourceCalendar>(x => x.Name.Contains(paged.Search)));
            var query = SearchQuery(spec.AsExpression());
            var items = await query.Skip(paged.Offset).Take(paged.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<ResourceCalendarBasic>(totalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResourceCalendarBasic>>(items)
            };
        }
    }
}
