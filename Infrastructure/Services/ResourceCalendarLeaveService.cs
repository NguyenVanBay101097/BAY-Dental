using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using Umbraco.Web.Models.ContentEditing;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class ResourceCalendarLeaveService : BaseService<ResourceCalendarLeaves>, IResourceCalendarLeaveService
    {
        private readonly IMapper _mapper;
        public ResourceCalendarLeaveService(IMapper mapper, IAsyncRepository<ResourceCalendarLeaves> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }
        public async Task<PagedResult2<ResourceCalendarLeaveDisplay>> GetPaged(ResourceCalendarLeavePaged val)
        {
            var query = SearchQuery();
            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<ResourceCalendarLeaveDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<ResourceCalendarLeaveDisplay>>(items)
            };
        }
    }
}
