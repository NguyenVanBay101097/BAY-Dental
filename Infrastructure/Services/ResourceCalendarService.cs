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
   public class ResourceCalendarService : BaseService<ResourceCalendar>, IResourceCalendarService
    {

        private readonly IMapper _mapper;
        public ResourceCalendarService(IAsyncRepository<ResourceCalendar> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<ResourceCalendar> GetResourceCalendarDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).FirstOrDefaultAsync();
            return res;
        }

        public async Task<IEnumerable<ResourceCalendar>> GetAll()
        {
            var query = SearchQuery();

            return await query.ToListAsync();
        }
    }
}
