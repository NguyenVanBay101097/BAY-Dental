﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using AutoMapper;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record.PivotTable;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<ChamCong> GetLastChamCong(employeePaged val)
        {
            var query = SearchQuery(x => x.EmployeeId == val.EmployeeId && x.Date == val.Date);
            if (query.Any())
                return await query.OrderBy(x => x.DateCreated).LastAsync();
            else
            {
                return null;
            }
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

        public int SoCongChuan(List<IGrouping<string, ResourceCalendarAttendance>> list, DateTime start, DateTime end)
        {
            int congChuan = 0;
            foreach (var att in list)
            {
                congChuan = congChuan + CountDays((DayOfWeek)Enum.Parse(typeof(DayOfWeek), att.Key), start, end);
            }
            for (int i = 0; i < congChuan; i++)
            {

            }
            return congChuan;
        }

        public CongEmplyee SoCongNhanVienTheoThang(List<ChamCong> listChamCongs, Dictionary<DateTime, List<ResourceCalendarAttendance>> re_listResourceCalendarAtts)
        {
            var congEmp = new CongEmplyee();
            foreach (var cc in listChamCongs)
            {

                congEmp.SoCong = congEmp.SoCong + TinhTien(cc, re_listResourceCalendarAtts).SoCong;
                congEmp.SoGioLam = congEmp.SoGioLam + TinhTien(cc, re_listResourceCalendarAtts).SoGioLam;


            }
            return congEmp;
        }

        public Dictionary<DateTime, List<ResourceCalendarAttendance>> SoNgayLam(DateTime? from, DateTime? to, List<ResourceCalendarAttendance> listAtts)
        {
            var listDic = new Dictionary<DateTime, List<ResourceCalendarAttendance>>();

            for (DateTime i = from.Value; i <= to.Value; i = i.AddDays(1.0))
            {
                var items = new List<ResourceCalendarAttendance>();
                foreach (var att in listAtts)
                {
                    if ((int)i.DayOfWeek == Int32.Parse(att.DayOfWeek))
                    {
                        items.Add(att);
                    }
                }
                if (items.Count() > 0)
                    listDic.Add(i, items);
            }



            return listDic;
        }

        public CongEmplyee TinhTien(ChamCong cc, Dictionary<DateTime, List<ResourceCalendarAttendance>> re_listResourceCalendarAtts)
        {
            var congEmp = new CongEmplyee();
            foreach (var att in re_listResourceCalendarAtts)
            {
                var ccTimeIn = cc.TimeIn.Value.TimeOfDay.TotalHours;
                var ccTimeOut = cc.TimeOut.Value.TimeOfDay.TotalHours;

                if (cc.Date.Value == att.Key)
                {
                    double soGio = 0;
                    var buoi = att.Value.FirstOrDefault();
                    var sogioTb1Ngay = buoi != null && buoi.Calendar != null ? buoi.Calendar.HoursPerDay : 0;
                    if (att.Value.Count() > 1)
                    {
                        foreach (var item in att.Value.ToList())
                        {
                            if (ccTimeIn <= item.HourFrom && ccTimeOut >= item.HourTo) //dungg gio hoac di som ve muon 
                                soGio = soGio + (item.HourTo - item.HourFrom);
                            else if (ccTimeIn > item.HourFrom) //di lam luon
                                soGio = soGio + (item.HourTo - ccTimeIn);
                            else if (ccTimeOut < item.HourTo) //ve som
                                soGio = soGio + (ccTimeOut - item.HourFrom);
                        }
                        var total = Decimal.Parse(soGio.ToString()) / sogioTb1Ngay.Value;
                        if (total >= (75 / 100) && total <= 1)
                            congEmp.SoCong++;
                        else
                        {
                            congEmp.SoCong = congEmp.SoCong + 1 / 2;
                        }
                        congEmp.SoGioLam = congEmp.SoGioLam + Decimal.Parse(soGio.ToString());

                    }
                    else if (att.Value != null && att.Value.Count() == 1)
                    {
                        if (ccTimeIn <= buoi.HourFrom && ccTimeOut >= buoi.HourTo)
                            soGio = soGio + (buoi.HourTo - buoi.HourFrom);
                        else if (ccTimeIn > buoi.HourFrom)
                            soGio = soGio + (buoi.HourTo - ccTimeIn);
                        else if (ccTimeOut < buoi.HourTo)
                            soGio = soGio + (ccTimeOut - buoi.HourFrom);

                        var total = Decimal.Parse(soGio.ToString()) / sogioTb1Ngay.Value;
                        if (total >= (75 / 100))
                            congEmp.SoCong++;
                        else
                        {
                            congEmp.SoCong = congEmp.SoCong + 1 / 2;
                        }
                        congEmp.SoGioLam = congEmp.SoGioLam + Decimal.Parse(soGio.ToString());
                    }
                }
            }
            return congEmp;
        }

        public CongEmplyee SoGioLamNhanVien(List<ChamCong> list, List<IGrouping<string, ResourceCalendarAttendance>> re_listResourceCalendarAtts)
        {
            var congEmp = new CongEmplyee();

            for (int i = 0; i < list.Count(); i++)
            {
                var cc = list[i];
                foreach (var att in re_listResourceCalendarAtts)
                {
                    if ((int)cc.Date.Value.DayOfWeek == Int32.Parse(att.Key))
                    {
                        congEmp.SoGioLam = Decimal.Parse(((cc.TimeOut.Value - cc.TimeIn.Value).TotalHours).ToString());
                        congEmp.SoCong++;
                    }
                }
            }
            return congEmp;
        }

        public async Task<CongEmplyee> CountCong(Guid? empId, DateTime? from, DateTime? to)
        {
            var structureTypeObj = GetService<IHrPayrollStructureTypeService>();
            var empObj = GetService<IEmployeeService>();
            var query = SearchQuery(x => x.EmployeeId == empId);
            var listResourceCalendarAtts = new List<ResourceCalendarAttendance>();
            var congEmp = new CongEmplyee();
            if (to.HasValue)
                query = query.Where(x => x.Date <= to.Value);
            if (from.HasValue)
                query = query.Where(x => x.Date >= from.Value);
            var emp = await empObj.SearchQuery(x => x.Id == empId).FirstOrDefaultAsync();
            var structureType = await structureTypeObj.SearchQuery(x => x.Id == emp.StructureTypeId)
                .Include("DefaultResourceCalendar")
                .Include("DefaultResourceCalendar.ResourceCalendarAttendances").FirstOrDefaultAsync();

            var listChamCongs = await query.Include(x => x.WorkEntryType).ToListAsync();
            if (structureType != null)
                listResourceCalendarAtts = structureType.DefaultResourceCalendar != null ? structureType.DefaultResourceCalendar.ResourceCalendarAttendances.ToList() : new List<ResourceCalendarAttendance>();

            var SoNgayLamTrongThang = SoNgayLam(from, to, listResourceCalendarAtts);
            var re_listResourceCalendarAtts = listResourceCalendarAtts.GroupBy(x => x.DayOfWeek).ToList();

            switch (structureType.WageType)
            {
                case "monthly":
                    congEmp = SoCongNhanVienTheoThang(listChamCongs, SoNgayLamTrongThang);
                    congEmp.CongChuan1Thang = SoNgayLamTrongThang.Count();
                    break;
                case "hourly":
                    congEmp = SoGioLamNhanVien(listChamCongs, re_listResourceCalendarAtts);
                    break;
                default:
                    break;
            }

            return congEmp;
        }

        public int CountDays(DayOfWeek day, DateTime start, DateTime end)
        {
            TimeSpan ts = end - start;
            int count = (int)Math.Floor(ts.TotalDays / 7);
            int remainder = (int)(ts.TotalDays % 7);
            int sinceLastDay = (int)(end.DayOfWeek - day);
            if (sinceLastDay < 0) sinceLastDay += 7;
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

        public async Task ImportExcel(PartnerImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ImportFileExcellChamCongModel>();
            var empObj = GetService<IEmployeeService>();
            var unitWorkObj = GetService<IUnitOfWorkAsync>();
            var workEntryTypeObj = GetService<IWorkEntryTypeService>();
            var errors = new List<string>();
            using (var stream = new MemoryStream(fileData))
            {
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string maNv = "";
                        string curDateString = "";
                        string code = "";
                        string de_time = Convert.ToString(worksheet.Cells[row, 3].Value);
                        var errs = new List<string>();
                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 1].Value)))
                            maNv = Convert.ToString(worksheet.Cells[row, 1].Value);
                        else
                        {
                            throw new Exception($"Dòng {row}: Mã nhân viên null");
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 2].Value)))
                            curDateString = Convert.ToString(worksheet.Cells[row, 2].Value);
                        else
                        {
                            throw new Exception($"Dòng {row}: Ngày chấm công null");
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 5].Value)))
                            code = Convert.ToString(worksheet.Cells[row, 5].Value);
                        else
                        {
                            throw new Exception($"Dòng {row}: loại chấm công null");
                        }

                        var time = $"{DateTime.Parse(curDateString).ToShortDateString()} { DateTime.Parse(de_time).ToShortTimeString()}";
                        try
                        {
                            data.Add(new ImportFileExcellChamCongModel
                            {
                                Stt = row,
                                MaNV = maNv,
                                Date = DateTime.Parse(curDateString),
                                Time = DateTime.Parse(time),
                                Type = Convert.ToString(worksheet.Cells[row, 4].Value),
                                CodeWorkEntryType = code,
                            });
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                }
            }

            if (data.Count > 0)
            {
                var res = data.GroupBy(x => x.MaNV).ToList();
                for (int i = 0; i < res.Count(); i++)
                {
                    var item = res[i];
                    var emp = await empObj.SearchQuery(x => x.Ref.Contains(item.Key)).FirstOrDefaultAsync();
                    var workEntryType = new WorkEntryType();
                    int count = 1;
                    var chamcong = new ChamCong();
                    foreach (var cc in item.ToList())
                    {
                        try
                        {
                            if (cc.Type == "check-in")
                            {
                                chamcong.Id = new Guid();
                                workEntryType = await workEntryTypeObj.SearchQuery(x => x.Code.Contains(cc.CodeWorkEntryType)).FirstOrDefaultAsync();
                                chamcong.CompanyId = CompanyId;
                                chamcong.EmployeeId = emp.Id;
                                chamcong.Date = cc.Date;
                                chamcong.WorkEntryTypeId = workEntryType.Id;
                                chamcong.TimeIn = cc.Time;
                                if (count == item.Count())
                                {
                                    chamcong.Status = "process";
                                    await CheckChamCong(chamcong, cc.Stt.ToString());
                                    await unitWorkObj.BeginTransactionAsync();
                                    await CreateAsync(chamcong);
                                    unitWorkObj.Commit();
                                }
                            }
                            else if (cc.Type == "check-out")
                            {
                                chamcong.TimeOut = cc.Time;
                                if (chamcong.TimeIn.HasValue && chamcong.TimeOut.HasValue)
                                    chamcong.Status = "done";
                                else if (chamcong.TimeIn.HasValue || chamcong.TimeOut.HasValue)
                                    chamcong.Status = "process";
                                await CheckChamCong(chamcong, cc.Stt.ToString());
                                await unitWorkObj.BeginTransactionAsync();
                                await CreateAsync(chamcong);
                                unitWorkObj.Commit();
                                chamcong = new ChamCong();
                            }
                            count++;
                        }
                        catch (Exception e)
                        {

                            throw new Exception(e.Message);
                        }

                    }
                }
            }
        }

        public async Task CheckChamCong(ChamCong val, string stt)
        {
            if (val == null)
                throw new Exception("Input null");
            else
            {
                string message = "";

                if (!val.TimeIn.HasValue)
                {
                    message = (!string.IsNullOrEmpty(stt) ? $"Dòng thứ {stt} : " : null) + "Thời gian vào không được để trống ";
                    throw new Exception(message);
                }

                if (val.TimeIn > val.TimeOut)
                {
                    message = (!string.IsNullOrEmpty(stt) ? $"Dòng thứ {stt} : " : null) + "Thời gian vào không được lớn hơn thời gian ra";
                    throw new Exception(message);
                }

                var ccFirst = new ChamCong();
                var query = SearchQuery(x => x.EmployeeId == val.EmployeeId && x.TimeIn.Value.Date == val.TimeIn.Value.Date);
                if (query.Any())
                {
                    ccFirst = await query.OrderBy(x => x.DateCreated).LastOrDefaultAsync();
                }
                else
                {
                    ccFirst = null;
                }

                if (ccFirst != null && val.Id != ccFirst.Id && !ccFirst.TimeOut.HasValue)
                {
                    message = (!string.IsNullOrEmpty(stt) ? $"Dòng thứ {stt} : " : null) + "Bạn chưa hoàn thành 1 chấm công trước đó, hoàn thành chấm công sau đó tiếp tục thao tác lại ";
                    throw new Exception(message);
                }
                if (ccFirst != null && val.Id != ccFirst.Id && val.TimeIn < ccFirst.TimeOut)
                {
                    message = (!string.IsNullOrEmpty(stt) ? $"Dòng thứ {stt} : " : null) + "Thời gian vào của châm công này phải lớn hơn thời gian ra của chấm công trước đó ";
                    throw new Exception(message);
                }
            }
        }

        public override async Task<IEnumerable<ChamCong>> CreateAsync(IEnumerable<ChamCong> entities)
        {
            foreach (var entity in entities)
            {
                await CheckChamCong(entity, null);
            }
            return await base.CreateAsync(entities);
        }

        public override async Task UpdateAsync(IEnumerable<ChamCong> entities)
        {
            foreach (var entity in entities)
            {
                await CheckChamCong(entity, null);
            }
            await base.UpdateAsync(entities);
        }
    }
}
