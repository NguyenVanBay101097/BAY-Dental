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
            var res = await SearchQuery(x => x.Id == id).Include(x=>x.ResourceCalendarAttendances).FirstOrDefaultAsync();
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

        public async Task<ResourceCalendar> CreateResourceCalendar(ResourceCalendarSave val)
        {
            var resourceCalendar = _mapper.Map<ResourceCalendar>(val);
            resourceCalendar.CompanyId = CompanyId;

            SaveAttendances(val, resourceCalendar);

            return await CreateAsync(resourceCalendar);
        }

        public async Task UpdateResourceCalendar(Guid id, ResourceCalendarSave val)
        {
            var resourceCalendar = await SearchQuery(x => x.Id == id).Include(x => x.ResourceCalendarAttendances).FirstOrDefaultAsync();
            if (resourceCalendar == null)
                throw new Exception("Lịch làm việc không tồn tại");

            resourceCalendar = _mapper.Map(val, resourceCalendar);
            SaveAttendances(val, resourceCalendar);

            await UpdateAsync(resourceCalendar);
        }


        private void SaveAttendances(ResourceCalendarSave val, ResourceCalendar resourceCalendar)
        {
            //remove line
            var lineToRemoves = new List<ResourceCalendarAttendance>();
            foreach (var existLine in resourceCalendar.ResourceCalendarAttendances)
            {
                if (!val.ResourceCalendarAttendances.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                resourceCalendar.ResourceCalendarAttendances.Remove(line);
            }

            int sequence = 0;
            foreach (var line in val.ResourceCalendarAttendances)
                line.Sequence = sequence++;

            foreach (var line in val.ResourceCalendarAttendances)
            {
                if (line.Id == Guid.Empty)
                {
                    resourceCalendar.ResourceCalendarAttendances.Add(_mapper.Map<ResourceCalendarAttendance>(line));
                }
                else
                {
                    _mapper.Map(line, resourceCalendar.ResourceCalendarAttendances.SingleOrDefault(c => c.Id == line.Id));
                }
            }

        }
    }
}
