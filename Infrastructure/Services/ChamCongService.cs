using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record.PivotTable;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class ChamCongService : BaseService<ChamCong>, IChamCongService
    {
        private readonly IMapper _mapper;
        public ChamCongService(IAsyncRepository<ChamCong> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task CreateListChamcongs(IEnumerable<ChamCong> val)
        {
            var companyId = this.CompanyId;
            foreach (var item in val)
            {
                item.CompanyId = companyId;
            }
            await this.CreateAsync(val);
        }
        //public async Task<PagedResult2<EmployeeDisplay>> GetByEmployeePaged(employeePaged val)
        //{
        //    ISpecification<Employee> spec = new InitialSpecification<Employee>(x => true);
        //    if (!string.IsNullOrEmpty(val.Filter))
        //    {
        //        spec = spec.And(new InitialSpecification<Employee>(x => x.Name.Contains(val.Filter)));
        //    }

        //    var empObj = GetService<IEmployeeService>();
        //    var query = empObj.SearchQuery().Include(x => x.ChamCongs).Include("ChamCongs.WorkEntryType").
        //        Select(x => new Employee()
        //        {
        //            Id = x.Id,
        //            CompanyId = x.CompanyId,
        //            Name = x.Name,
        //            Ref = x.Ref,
        //            ChamCongs = x.ChamCongs.Where(y => (!val.From.HasValue || (y.TimeIn.Value.Date >= val.From.Value.Date || y.TimeOut.Value.Date >= val.From.Value.Date))
        //            &&
        //            (!val.To.HasValue ||
        //            (y.TimeIn.Value.Date <= val.To.Value.Date || y.TimeOut.Value.Date <= val.To.Value.Date))
        //            &&
        //            (string.IsNullOrEmpty(val.Status) ||
        //            (y.Status == val.Status))

        //        ).Select(y => new ChamCong()
        //        {
        //            Id = y.Id,
        //            DateCreated = y.DateCreated,
        //            Status = y.Status,
        //            WorkEntryTypeId = y.WorkEntryTypeId,
        //            Date = y.Date,
        //            TimeIn = y.TimeIn,
        //            TimeOut = y.TimeOut,
        //            WorkEntryType = y.WorkEntryType,
        //            HourWorked = y.HourWorked
        //        }).ToList()
        //        });

        //    var items = await query.Skip(val.Offset).Take(val.Limit)
        //       .ToListAsync();

        //    var totalItems = await query.CountAsync();
        //    var resItems = _mapper.Map<IEnumerable<EmployeeDisplay>>(items);
        //    resItems = await CountNgayCong(resItems);
        //    return new PagedResult2<EmployeeDisplay>(totalItems, val.Offset, val.Limit)
        //    {
        //        Items = resItems
        //    };
        //}

        public async Task<IEnumerable<EmployeeDisplay>> CountNgayCong(IEnumerable<EmployeeDisplay> lst)
        {
            var setupObj = GetService<ISetupChamcongService>();
            var setup = await setupObj.GetByCompanyId();
            //skipp if setup null
            if (setup == null)
            {
                return lst;
            }
            var diffTime = Math.Round(setup.DifferenceTime / 60, 2);
            var fromOne = setup.OneStandardWorkHour - diffTime;
            var fromHalf = setup.HalfStandardWorkHour - diffTime;
            double Tongcong = 0;
            foreach (var emp in lst)
            {
                Tongcong = 0;
                foreach (var chamcong in emp.ChamCongs)
                {
                    if (chamcong.HourWorked >= fromOne || (chamcong.WorkEntryType != null && chamcong.WorkEntryType.IsHasTimeKeeping))
                    {
                        Tongcong += 1;
                    }
                    else if (chamcong.HourWorked >= fromHalf)
                    {
                        Tongcong += 0.5;
                    }
                }
                emp.SoNgayCong = Tongcong;
            }
            return lst;
        }

        public async Task<string> GetStatus(ChamCong val)
        {
            if (val.TimeIn.HasValue && val.TimeOut.HasValue)
            {
                return "done";
            }
            else if (!val.TimeOut.HasValue && !val.TimeIn.HasValue)
            {
                var workEntry = await GetService<IWorkEntryTypeService>().GetByIdAsync(val.WorkEntryTypeId);
                if (workEntry == null)
                {
                    throw new Exception("loại chấm công không tìm thấy");
                }
                if (workEntry.IsHasTimeKeeping)
                {
                    return "NP";
                }
                return "Initial";
            }
            return "process";
        }
        public async Task<ChamCongDisplay> GetByEmployeeId(Guid id, DateTime date)
        {
            var chamcong = await SearchQuery(x => x.EmployeeId == id
            && (x.TimeIn.Value.Date.Equals(date.Date) || x.TimeOut.Value.Date.Equals(date.Date))).Include(x => x.Employee)
                .Include(x => x.WorkEntryType)
                .FirstOrDefaultAsync();
            return _mapper.Map<ChamCongDisplay>(chamcong);
        }

        public async Task<decimal> GetStandardWorkHour()
        {
            var st = await GetService<ISetupChamcongService>().GetByCompanyId(CompanyId);
            if (st == null)
            {
                throw new Exception("cần setup chấm công trước");
            }
            return st.OneStandardWorkHour;
        }

        //public async Task<IEnumerable<ChamCongDisplay>> ExportFile(employeePaged val)
        //{
        //    int dateStart = 0;
        //    int dateEnd = 0;
        //    var listDateCC = new List<DateChamCong>();
        //    var emps = await GetByEmployeePaged(val);
        //    var dictEmp = new Dictionary<Guid, List<DateChamCong>>();
        //    if (val.From.HasValue)
        //        dateStart = val.From.Value.Day;
        //    if (val.To.HasValue)
        //        dateEnd = val.To.Value.Day;

        //    for (int i = dateStart; i <= dateEnd; i++)
        //    {
        //        foreach (var emp in emps.Items)
        //        {
        //            if (emp.ChamCongs.Count() > 0)
        //            {
        //                foreach (var cc in emp.ChamCongs)
        //                {
        //                    if (cc.DateCreated.HasValue && cc.DateCreated.Value.Day == i)
        //                    {
        //                        var dateCC = new DateChamCong();
        //                        dateCC.Date = cc.DateCreated.Value;
        //                        dateCC.ChamCong = cc;
        //                        listDateCC.Add(dateCC);
        //                    }
        //                }
        //                dictEmp.Add(emp.Id, listDateCC);
        //            }
        //            //else
        //            //{
        //            //    var datec = new DateChamCong();
        //            //    datec.Date = new DateTime(currentYear, currentMonth, i);
        //            //    dictEmp[emp].Add(datec);
        //            //}
        //        }
        //    }


        //    return null;
        //}

        public async Task<IEnumerable<ChamCongDisplay>> GetAll(employeePaged val)
        {
            var query = SearchQuery();
            if (val.From.HasValue)
            {
                query = query.Where(y => y.Date.Value.Date >= val.From.Value.Date);
            }

            if (val.To.HasValue)
            {
                query = query.Where(y => y.Date.Value.Date <= val.To.Value.Date);
            }

            if (!string.IsNullOrEmpty(val.Status))
            {
                query = query.Where(x => x.Status.Equals(val.Status));
            }

            if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }

            var res = await query.Include(x => x.WorkEntryType).ToListAsync();
            return _mapper.Map<IEnumerable<ChamCongDisplay>>(res);
        }

        public async Task<CongEmplyee> CountCong(Guid? empId, DateTime? from, DateTime? to)
        {
            var structureTypeObj = GetService<IHrPayrollStructureTypeService>();
            var empObj = GetService<IEmployeeService>();
            var query = SearchQuery(x => x.EmployeeId == empId);
            var listResourceCalendarAtts = new List<ResourceCalendarAttendance>();
            var congEmp = new CongEmplyee();
            if (to.HasValue)
                query = query.Where(x => x.Date <= to);
            if (from.HasValue)
                query = query.Where(x => x.Date >= from);

            var emp = await empObj.SearchQuery(x => x.Id == empId).FirstOrDefaultAsync();
            var structureType = await structureTypeObj.SearchQuery(x => x.Id == emp.StructureTypeId)
                .Include("DefaultResourceCalendar")
                .Include("DefaultResourceCalendar.ResourceCalendarAttendances").FirstOrDefaultAsync();

            var listChamCongs = await query.Include(x => x.WorkEntryType).ToListAsync();
            if (structureType != null)
                listResourceCalendarAtts = structureType.DefaultResourceCalendar != null ? structureType.DefaultResourceCalendar.ResourceCalendarAttendances.ToList() : new List<ResourceCalendarAttendance>();

            foreach (var att in listResourceCalendarAtts)
            {
                congEmp.CongChuan1Thang = congEmp.CongChuan1Thang + CountDays(((DayOfWeek)Enum.Parse(typeof(DayOfWeek), att.DayOfWeek)), from.Value, to.Value);
            }

            foreach (var cc in listChamCongs)
            {
                if (!cc.WorkEntryType.IsHasTimeKeeping)
                    continue;
                foreach (var att in listResourceCalendarAtts)
                {

                    if ((int)cc.Date.Value.DayOfWeek == Int32.Parse(att.DayOfWeek))
                    {
                        var ccTimeIn = cc.TimeIn.Value.TimeOfDay.TotalHours;
                        var ccTimeOut = cc.TimeOut.Value.TimeOfDay.TotalHours;

                        if (ccTimeIn <= att.HourFrom && ccTimeOut >= att.HourTo)
                            congEmp.SoCong++;
                        else if (Decimal.Parse((ccTimeOut - ccTimeIn).ToString()) >= structureType.DefaultResourceCalendar.HoursPerDay.Value)
                            congEmp.SoCong++;
                        else if (Decimal.Parse((ccTimeOut - ccTimeIn).ToString()) >= structureType.DefaultResourceCalendar.HoursPerDay.Value / 2)
                            congEmp.SoCong = congEmp.SoCong + 1 / 2;
                    }
                    //else
                    //{
                    //    if ((int)cc.Date.Value.DayOfWeek == 0 || (int)cc.Date.Value.DayOfWeek == 6)
                    //    //{
                    //    //    var ccTimeIn = cc.TimeIn.Value.TimeOfDay.TotalHours;
                    //    //    var ccTimeOut = cc.TimeOut.Value.TimeOfDay.TotalHours;

                    //    //    if (ccTimeIn <= att.HourFrom && ccTimeOut >= att.HourTo)
                    //    //        congEmp.SoCong++;
                    //    //    else if (ccTimeIn > att.HourFrom && ccTimeOut < att.HourTo)
                    //    //        if (Decimal.Parse((ccTimeOut - ccTimeIn).ToString()) >= structureType.DefaultResourceCalendar.HoursPerDay.Value)
                    //    //            congEmp.SoCong++;
                    //    //}
                    //}
                }
            }

            return congEmp;
        }

        public int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;                       // Total duration
            int count = (int)Math.Floor(ts.TotalDays / 7);   // Number of whole weeks
            int remainder = (int)(ts.TotalDays % 7);         // Number of remaining days
            int sinceLastDay = (int)(end.DayOfWeek - day);   // Number of days since last [day]
            if (sinceLastDay < 0) sinceLastDay += 7;         // Adjust for negative days since last [day]

            // If the days in excess of an even week are greater than or equal to the number days since the last [day], then count this one, too.
            if (remainder >= sinceLastDay) count++;

            return count;
        }

        public ChamCongTinhCong TinhSoCongGioCong(ChamCong cc, IEnumerable<AttendanceInterval> attendanceIntervals, decimal hoursPerDay)
        {
            var dict = new Dictionary<DateTime, IList<AttendanceInterval>>();
            foreach (var item in attendanceIntervals)
            {
                if (!dict.ContainsKey(item.Start.Date))
                    dict.Add(item.Start.Date, new List<AttendanceInterval>());
                dict[item.Start.Date].Add(item);
            }

            var intervals = dict.ContainsKey(cc.TimeIn.Value.Date) ? dict[cc.TimeIn.Value.Date] : new List<AttendanceInterval>();
            double gioCong = 0;
            foreach (var interval in intervals)
            {
                //nếu chấm công giờ vào và giờ ra hoàn toàn nằm ngoài chặn dưới và chặn trên thì bỏ qua
                if (cc.TimeOut <= interval.Start || cc.TimeIn >= interval.Stop)
                    continue;

                //nếu chấm công vào mà nhỏ hơn chặn dưới thì lấy chặn dưới, còn lớn hơn thì lấy chấm công vào
                //dùng dt0 để lưu lại
                var dt0 = cc.TimeIn < interval.Start ? interval.Start : cc.TimeIn;
                //nếu chấm công ra mà lớn hơn chặn trên thì lấy chặn trên, ngược lại thì lấy chấm công ra
                //dùng dt1 để lưu lại
                var dt1 = cc.TimeOut > interval.Stop ? interval.Stop : cc.TimeOut;
                //giờ làm việc = (dt1 - dt0) quy sang giờ
                //sogio += giờ làm việc
                var actualHours = (dt1 - dt0).Value.TotalSeconds / 3600;
                gioCong += actualHours;
            }

            var soNgayCong = Math.Round(gioCong / (double)hoursPerDay, 2);

            return new ChamCongTinhCong
            {
                SoGioCong = gioCong,
                SoNgayCong = soNgayCong
            };
        }

    }
}
