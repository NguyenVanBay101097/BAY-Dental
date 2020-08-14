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
    public class ResourceCalendarAttendanceService : BaseService<ResourceCalendarAttendance>, IResourceCalendarAttendanceService
    {
        private readonly IMapper _mapper;
        public ResourceCalendarAttendanceService(IMapper mapper, IAsyncRepository<ResourceCalendarAttendance> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<ResourceCalendarAttendanceBasic>> GetPaged(ResourceCalendarAttendancePaged paged)
        {
            ISpecification<ResourceCalendarAttendance> spec = new InitialSpecification<ResourceCalendarAttendance>(x => true);
            if (!string.IsNullOrEmpty(paged.Search))
                spec = spec.And(new InitialSpecification<ResourceCalendarAttendance>(x => x.Name.Contains(paged.Search)));
            var query = SearchQuery(spec.AsExpression());
            var items = await query.Skip(paged.Offset).Take(paged.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<ResourceCalendarAttendanceBasic>(totalItems, paged.Offset, paged.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResourceCalendarAttendanceBasic>>(items)
            };
        }
    }
}
