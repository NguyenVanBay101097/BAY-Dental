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

        public async Task<IEnumerable<AttendanceInterval>> _AttendanceIntervals(Guid id, DateTime start_dt, DateTime end_dt)
        {
            var self = await SearchQuery(x => x.Id == id).Include(x => x.ResourceCalendarAttendances).FirstOrDefaultAsync();
            var result = new List<AttendanceInterval>();
            foreach (var attendance in self.ResourceCalendarAttendances)
            {
                var start = start_dt.Date;
                var until = end_dt.Date;
                var weekday = Convert.ToInt32(attendance.DayOfWeek);

                foreach (var day in GetListDate(start, until, weekday))
                {
                    var dt0 = day.AddHours(attendance.HourFrom);
                    var dt1 = day.AddHours(attendance.HourTo);
                    result.Add(new AttendanceInterval
                    {
                        Start = dt0,
                        Stop = dt1,
                        Attendance = attendance
                    });
                }
            }

            return result;
        }

        public IEnumerable<DateTime> GetListDate(DateTime start_dt, DateTime end_dt, int weekday)
        {
            var res = new List<DateTime>();
            var date = start_dt;
            while (date <= end_dt)
            {
                if ((int)date.DayOfWeek == weekday)
                    res.Add(date);
                date = date.AddDays(1);
            }

            return res;
        }
    }

    public class AttendanceInterval
    {
        public DateTime Start { get; set; }

        public DateTime Stop { get; set; }

        public ResourceCalendarAttendance Attendance { get; set; }
    }
}
