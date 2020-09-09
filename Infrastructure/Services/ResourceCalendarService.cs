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
using System.Globalization;
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
            var leaveObj = GetService<IResourceCalendarLeaveService>();

            res.Attendances = await _mapper.ProjectTo<ResourceCalendarAttendanceDisplay>(attendanceObj.SearchQuery(x => x.CalendarId == id, orderBy: x => x.OrderBy(s => s.DayOfWeek).ThenBy(s => s.HourFrom))).ToListAsync();
            res.Leaves = await _mapper.ProjectTo<ResourceCalendarLeaveDisplay>(leaveObj.SearchQuery(x => x.CalendarId == id, orderBy: x => x.OrderBy(s => s.DateFrom))).ToListAsync();
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
            var self = await SearchQuery(x => x.Id == id).Include(x => x.Attendances).FirstOrDefaultAsync();
            var result = new List<AttendanceInterval>();
            foreach (var attendance in self.Attendances)
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

            SaveAttendances(val, resourceCalendar);
            SaveLeaves(val, resourceCalendar);

            return await CreateAsync(resourceCalendar);
        }

        public async Task UpdateResourceCalendar(Guid id, ResourceCalendarSave val)
        {
            var resourceCalendar = await SearchQuery(x => x.Id == id).Include(x => x.Attendances).Include(x=>x.Leaves).FirstOrDefaultAsync();
            if (resourceCalendar == null)
                throw new Exception("Lịch làm việc không tồn tại");

            resourceCalendar = _mapper.Map(val, resourceCalendar);
            SaveAttendances(val, resourceCalendar);
            SaveLeaves(val, resourceCalendar);

            await UpdateAsync(resourceCalendar);
        }

        private void SaveAttendances(ResourceCalendarSave val, ResourceCalendar resourceCalendar)
        {
            //remove line
            var lineToRemoves = new List<ResourceCalendarAttendance>();
            foreach (var existLine in resourceCalendar.Attendances)
            {
                if (!val.Attendances.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                resourceCalendar.Attendances.Remove(line);
            }

            foreach (var line in val.Attendances)
            {
                if (line.Id == Guid.Empty)
                {
                    resourceCalendar.Attendances.Add(_mapper.Map<ResourceCalendarAttendance>(line));
                }
                else
                {
                    _mapper.Map(line, resourceCalendar.Attendances.SingleOrDefault(c => c.Id == line.Id));
                }
            }

        }

        private void SaveLeaves(ResourceCalendarSave val, ResourceCalendar resourceCalendar)
        {
            //detect overlap
            DetectOverLapLeaves(_mapper.Map<List<ResourceCalendarLeaves>>(val.Leaves));
            //remove leaves
            var lineToRemoves = new List<ResourceCalendarLeaves>();
            foreach (var existLine in resourceCalendar.Leaves)
            {
                if (!val.Attendances.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                resourceCalendar.Leaves.Remove(line);
            }

            foreach (var line in val.Leaves)
            {
                if (line.Id == Guid.Empty)
                {
                    resourceCalendar.Leaves.Add(_mapper.Map<ResourceCalendarLeaves>(line));
                }
                else
                {
                    _mapper.Map(line, resourceCalendar.Leaves.SingleOrDefault(c => c.Id == line.Id));
                }
            }

        }

        private void DetectOverLapLeaves(List<ResourceCalendarLeaves> leaves)
        {
            for (int i = 0; i < leaves.Count - 1; i++)
            {
                for (int j = i+1; j < leaves.Count; j++)
                {
                    //nếu ko overlap thì tiếp tục
                    if (leaves[i].DateTo.Date < leaves[j].DateFrom.Date || leaves[i].DateFrom.Date > leaves[j].DateTo.Date)
                        continue;
                    else throw new Exception($"Lịch nghỉ {leaves[i].Name} trùng lịch với {leaves[j].Name}");
                }
            }
        }

        public ResourceCalendarDisplay DefaultGet()
        {
            var res = new ResourceCalendarDisplay();
            res.CompanyId = CompanyId;
            res.HoursPerDay = 8;

            var attendances = new List<ResourceCalendarAttendanceDisplay>()
            {
             new ResourceCalendarAttendanceDisplay("Sáng thứ 2","1",8,12,"morning"),
             new ResourceCalendarAttendanceDisplay("Chiều thứ 2","1",13,17,"afternoon"),
             new ResourceCalendarAttendanceDisplay("Sáng thứ 3","2",8,12,"morning"),
             new ResourceCalendarAttendanceDisplay("Chiều thứ 3","2",13,17,"afternoon"),
             new ResourceCalendarAttendanceDisplay("Sáng thứ 4","3",8,12,"morning"),
             new ResourceCalendarAttendanceDisplay("Chiều thứ 4","3",13,17,"afternoon"),
             new ResourceCalendarAttendanceDisplay("Sáng thứ 5","4",8,12,"morning"),
             new ResourceCalendarAttendanceDisplay("Chiều thứ 5","4",13,17,"afternoon"),
             new ResourceCalendarAttendanceDisplay("Sáng thứ 6","5",8,12,"morning"),
             new ResourceCalendarAttendanceDisplay("Chiều thứ 6","5",13,17,"afternoon"),
            };

            res.Attendances = attendances;
            return res;
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
