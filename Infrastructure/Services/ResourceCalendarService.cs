using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Mapping;
using Umbraco.Web.Models;
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

        public async Task<ResourceCalendarDisplay> GetDisplayAsync(Guid id)
        {
            var res = await _mapper.ProjectTo<ResourceCalendarDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            var attendanceObj = GetService<IResourceCalendarAttendanceService>();
            res.Attendances = await _mapper.ProjectTo<ResourceCalendarAttendanceDisplay>(attendanceObj.SearchQuery(x => x.CalendarId == id, orderBy: x => x.OrderBy(s => s.DayOfWeek).ThenBy(s => s.HourFrom))).ToListAsync();
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

        public async Task<ResourceCalendarNgayGioCongChuan> TinhSoCongChuan(Guid id, DateTime start_dt, DateTime end_dt, IEnumerable<AttendanceInterval> intervals = null)
        {
            //tra ve so ngay cong chuan va so gio cong chuan
            var res = new ResourceCalendarNgayGioCongChuan();
            //tao ra attendance
            intervals = intervals ?? await _AttendanceIntervals(id, start_dt, end_dt);

            //loop sum so gio
            double soGioCong = 0;
            foreach (var interval in intervals)
            {
                soGioCong += (interval.Stop - interval.Start).TotalSeconds / 3600;
            }
            //tra ve so ngay cong = so gio / hourPerdays va so gio sum
            var calendar = await GetByIdAsync(id);
            double soNgayCong = soGioCong / (double)calendar.HoursPerDay.Value;

            res.SoGioCong = soGioCong;
            res.SoNgayCong = soNgayCong;
            return res;
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

    public class AttendanceInterval
    {
        public DateTime Start { get; set; }

        public DateTime Stop { get; set; }

        public ResourceCalendarAttendance Attendance { get; set; }
    }

    public class ResourceCalendarNgayGioCongChuan
    {
        public double SoNgayCong { get; set; }
        public double SoGioCong { get; set; }
    }
}
