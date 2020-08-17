using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
   public class ResourceCalendarAttendanceService : BaseService<ResourceCalendarAttendance>, IResourceCalendarAttendanceService
    {

        private readonly IMapper _mapper;
        public ResourceCalendarAttendanceService(IAsyncRepository<ResourceCalendarAttendance> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<ResourceCalendarAttendance>> GetAll()
        {
            var query =  SearchQuery();
            return await query.ToListAsync();
        }

        public async Task<ResourceCalendarAttendance> GetResourceCalendarAttendanceDisplay(Guid id)
        {
            var res = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            return res;
        }
    }
}
