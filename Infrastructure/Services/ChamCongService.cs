using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using NPOI.HSSF.Record.PivotTable;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
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

        public async Task<PagedResult2<ChamCongDisplay>> GetPaged(ChamCongPaged val)
        {
            var query = SearchQuery(x => true);
            if (val.From.HasValue)
                query = query.Where(x => x.TimeIn >= val.From);
            if (val.To.HasValue)
                query = query.Where(x => x.TimeIn <= val.To);

            var totalItems = await query.CountAsync();

            if (val.Limit > 0)
                query = query.Take(val.Limit).Skip(val.Offset);

            var items = _mapper.Map<IEnumerable<ChamCongDisplay>>(await query.Include(x => x.WorkEntryType).ToListAsync());

            return new PagedResult2<ChamCongDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
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
                listResourceCalendarAtts = structureType.DefaultResourceCalendar != null ? structureType.DefaultResourceCalendar.Attendances.ToList() : new List<ResourceCalendarAttendance>();

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
                //nếu chấm công giờ vào và giờ ra hoàn toàn nằm ngoài chặn dưới và chặn trên hoặc chưa checkout thì bỏ qua
                if (cc.TimeOut <= interval.Start || cc.TimeIn >= interval.Stop || !cc.TimeOut.HasValue)
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

            var soNgayCong = gioCong / (double)hoursPerDay;

            var workEntryType = cc.WorkEntryType;
            if (workEntryType.RoundDays == "FULL")
                soNgayCong = FloatUtils.FloatRound(soNgayCong, precisionDigits: 0, roundingMethod: workEntryType.RoundDaysType);
            else if (workEntryType.RoundDays == "HALF")
            {
                var temp = soNgayCong / 5;
                soNgayCong = FloatUtils.FloatRound(temp, precisionDigits: 1, roundingMethod: workEntryType.RoundDaysType) * 5;
            }
            else
            {
                soNgayCong = Math.Round(soNgayCong, 2);
            }


            return new ChamCongTinhCong
            {
                SoGioCong = gioCong,
                SoNgayCong = soNgayCong
            };
        }

        public async Task<ChamCongImportResponse> ImportExcel(PartnerImportExcelViewModel val)
        {
            var fileData = Convert.FromBase64String(val.FileBase64);
            var data = new List<ImportFileExcellChamCongModel>();
            var listCC = new List<ChamCong>();
            var empObj = GetService<IEmployeeService>();
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
                        string time = "";
                        string type = "";
                        string de_time = Convert.ToString(worksheet.Cells[row, 3].Value);
                        var errs = new List<string>();

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 1].Value)))
                            maNv = Convert.ToString(worksheet.Cells[row, 1].Value);
                        else
                        {
                            errors.Add($"Dòng {row}: Mã nhân viên không được để trống");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 2].Value)))
                        {
                            curDateString = Convert.ToString(worksheet.Cells[row, 2].Value);
                        }
                        else
                        {
                            errors.Add($"Dòng {row}: Ngày chấm công không được để trống");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 5].Value)))
                            code = Convert.ToString(worksheet.Cells[row, 5].Value);
                        else
                        {
                            errors.Add($"Dòng {row}: Mã loại chấm công không được để trống");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 4].Value)))
                            type = Convert.ToString(worksheet.Cells[row, 4].Value);
                        else
                        {
                            errors.Add($"Dòng {row}: Vào/ra không được để trống");
                            continue;
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[row, 3].Value)))
                            time = $"{DateTime.Parse(curDateString):MM/dd/yyyy} { DateTime.Parse(de_time).ToShortTimeString()}";
                        else
                        {
                            errors.Add($"Dòng {row}: Thời gian không được trống");
                            continue;
                        }


                        try
                        {
                            data.Add(new ImportFileExcellChamCongModel
                            {
                                MaNV = maNv,
                                Date = DateTime.Parse(curDateString),
                                Time = DateTime.Parse(time),
                                Type = type,
                                CodeWorkEntryType = code,
                            });
                        }
                        catch (Exception e)
                        {
                            errors.Add($"Dòng {row}: {e.Message}");
                            continue;
                        }

                        if (errs.Any())
                        {
                            errors.Add($"Dòng {row}: {string.Join(", ", errs)}");
                            continue;
                        }
                    }
                }
            }
            if (errors.Any())
                return new ChamCongImportResponse { Success = false, Errors = errors };

            var employee_refs = data.Select(x => x.MaNV).Distinct().ToList();
            var word_entry_type_codes = data.Select(x => x.CodeWorkEntryType).Distinct().ToList();
            var emps = await empObj.SearchQuery(x => employee_refs.Contains(x.Ref)).ToListAsync();
            var works = await workEntryTypeObj.SearchQuery(x => word_entry_type_codes.Contains(x.Code)).ToListAsync();

            IDictionary<string, Employee> emp_dict = new Dictionary<string, Employee>();
            emp_dict = emps.ToDictionary(t => t.Ref, v => v);

            IDictionary<string, WorkEntryType> work_entry_type_dict = new Dictionary<string, WorkEntryType>();
            work_entry_type_dict = works.ToDictionary(t => t.Code, v => v);

            var errors_2 = new List<string>();

            foreach (var cc in data)
            {
                try
                {
                    if (!emp_dict.ContainsKey(cc.MaNV))
                        throw new Exception($"Không tìm thấy nhân viên mới mã {cc.MaNV}");

                    if (!work_entry_type_dict.ContainsKey(cc.CodeWorkEntryType))
                        throw new Exception($"Không loại công việc với mã {cc.CodeWorkEntryType}");
                    var employee = emp_dict[cc.MaNV];
                    if (cc.Type == "check-in")
                    {
                        var chamcong = new ChamCong();
                        chamcong.CompanyId = CompanyId;
                        chamcong.EmployeeId = emp_dict[cc.MaNV].Id;
                        chamcong.WorkEntryTypeId = work_entry_type_dict[cc.CodeWorkEntryType].Id;
                        chamcong.Date = cc.Date;
                        chamcong.TimeIn = cc.Time;
                        await CreateAsync(chamcong);
                    }
                    else if (cc.Type == "check-out")
                    {
                        var ccIn = await SearchQuery(x => x.EmployeeId == employee.Id && x.TimeOut == null).FirstOrDefaultAsync();
                        if (ccIn != null)
                        {
                            ccIn.TimeOut = cc.Time;
                            await UpdateAsync(ccIn);
                        }
                        else
                        {
                            throw new Exception($"Không thể check-out cho nhân viên {employee.Name}, không tìm thấy chấm công nào chưa check-out.");
                        }
                    }
                }
                catch (Exception e)
                {
                    errors_2.Add(e.Message);
                }
            }

            if (errors_2.Any())
                return new ChamCongImportResponse { Success = false, Errors = errors_2 };

            return new ChamCongImportResponse { Success = true };
        }

        public async Task<ResultSyncDataViewModel> SyncChamCong(IEnumerable<ReadLogResultDataViewModel> vals)
        {
            var resultSyncData = new ResultSyncDataViewModel();
            var modelError = new ImportFileExcellChamCongModel();
            var employeObj = GetService<IEmployeeService>();
            var unitObj = GetService<IUnitOfWorkAsync>();
            var workEntryTypeObj = GetService<IWorkEntryTypeService>();
            var workEntryType = await workEntryTypeObj.SearchQuery(orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
            if (workEntryType == null)
            {
                return new ResultSyncDataViewModel
                {
                    Success = false,
                    Message = "Không tìm thấy loại chấm công nào"
                };
            }

            var enrolls = vals.Select(x => x.UserId).Distinct().ToList();
            var emps = await employeObj.SearchQuery(x => !string.IsNullOrEmpty(x.EnrollNumber) && enrolls.Contains(x.EnrollNumber)).ToListAsync();
            var emp_dict = emps.GroupBy(x => x.EnrollNumber).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            int number_success = 0;
            int number_error = 0;
            foreach (var val in vals)
            {
                if (!emp_dict.ContainsKey(val.UserId))
                    continue;

                var employee = emp_dict[val.UserId];
                try
                {
                    ///nếu 2 checkin liên tiếp từ máy chấm công thì xóa tất cả cả chấm công mà chưa checkout trước
                    ///sau đó mới tạo chấm công
                    if (val.VerifyState == 0)
                    {
                        var cham_congs_not_check_out = await SearchQuery(x => x.EmployeeId == employee.Id && !x.TimeOut.HasValue).ToListAsync();
                        if (cham_congs_not_check_out.Any())
                            await DeleteAsync(cham_congs_not_check_out);

                        var chamcong = new ChamCong();
                        chamcong.CompanyId = CompanyId;
                        chamcong.EmployeeId = employee.Id;
                        chamcong.WorkEntryTypeId = workEntryType.Id;
                        chamcong.TimeIn = DateTime.Parse(val.VerifyDate);
                        await unitObj.BeginTransactionAsync();
                        await CreateAsync(chamcong);
                        unitObj.Commit();

                        number_success += 1;
                    }
                    ///check out
                    else if (val.VerifyState == 1)
                    {
                        var ccIn = await SearchQuery(x => x.EmployeeId == employee.Id && !x.TimeOut.HasValue).FirstOrDefaultAsync();
                        if (ccIn != null)
                        {
                            ccIn.TimeOut = DateTime.Parse(val.VerifyDate);
                            await unitObj.BeginTransactionAsync();
                            await UpdateAsync(ccIn);
                            unitObj.Commit();

                            number_success += 1;
                        }
                        else
                        {
                            number_error += 1;
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    number_error += 1;
                    unitObj.Rollback();
                    continue;
                }
            }

            return new ResultSyncDataViewModel
            {
                Success = true,
                NumberSuccess = number_success,
                NumberError = number_error
            };
        }

        public async Task CheckChamCong(IEnumerable<ChamCong> vals)
        {
            var empObj = GetService<IEmployeeService>();
            foreach (var val in vals)
            {
                // Thời gian vào không được trống
                if (!val.TimeIn.HasValue)
                    throw new Exception("Thời gian vào không được bỏ trống.");

                // Thời giân vào không được lớn hơn thời gian ra 
                if (val.TimeOut.HasValue && val.TimeOut < val.TimeIn)
                    throw new Exception("Thời gian ra không được nhỏ hơn thời gian vào");

                // Khoảng thời gian của các chấm công không được trùng lên nhau
                var cc_truoc = await SearchQuery(x => x.EmployeeId == val.EmployeeId && val.TimeIn > x.TimeIn && x.Id != val.Id).OrderByDescending(x => x.TimeIn).FirstOrDefaultAsync();
                if (cc_truoc != null && cc_truoc.TimeOut.HasValue && cc_truoc.TimeOut > val.TimeIn)
                {
                    var emp = await empObj.GetByIdAsync(val.EmployeeId);
                    throw new Exception($"Không thể tạo chấm công cho nhân viên {emp.Name}, nhân viên này đã có check-in vào lúc {val.TimeIn.Value:dd/MM/yyyy HH:mm}");
                }

                if (!val.TimeOut.HasValue)
                {
                    // Không được phép có 2 chấm công chưa có giờ ra của 1 nhân viên 
                    var cc = await SearchQuery(x => x.EmployeeId == val.EmployeeId && !x.TimeOut.HasValue && val.Id != x.Id).FirstOrDefaultAsync();
                    if (cc != null)
                    {
                        var emp = await empObj.GetByIdAsync(val.EmployeeId);
                        throw new Exception($"Không thể tạo chấm công cho nhân viên {emp.Name}, nhân viên này chưa check-out từ lúc {cc.TimeIn.Value:dd/MM/yyyy HH:mm}");
                    }
                }
                else
                {
                    var cc_sau = await SearchQuery(x => x.EmployeeId == val.EmployeeId && x.TimeIn < val.TimeOut && x.Id != val.Id).OrderByDescending(x => x.TimeIn).FirstOrDefaultAsync();
                    if (cc_sau != null && cc_sau != cc_truoc)
                    {
                        var emp = await empObj.GetByIdAsync(val.EmployeeId);
                        throw new Exception($"Không thể tạo chấm công cho nhân viên {emp.Name}, nhân viên này đã có check-in vào lúc ({val.TimeIn.Value:dd/MM/yyyy HH:mm})");
                    }
                }
            }

        }

        public void _ComputeStatusChamCong(IEnumerable<ChamCong> vals)
        {
            foreach (var val in vals)
            {
                if (val.TimeOut.HasValue)
                    val.Status = "done";
                else
                    val.Status = "process";
            }
        }

        public void _ComputeSoGioMotCong(IEnumerable<ChamCong> vals)
        {
            foreach (var val in vals)
            {
                if (val.TimeIn.HasValue && val.TimeOut.HasValue)
                    val.HourWorked = Math.Round((decimal)(val.TimeOut.Value - val.TimeIn.Value).TotalHours, 2);
            }
        }

        public override async Task<IEnumerable<ChamCong>> CreateAsync(IEnumerable<ChamCong> entities)
        {
            await base.CreateAsync(entities);
            await CheckChamCong(entities);
            return entities;
        }

        public override async Task UpdateAsync(IEnumerable<ChamCong> entities)
        {
            await base.UpdateAsync(entities);
            await CheckChamCong(entities);
        }

        public async Task<ChamCongDefaultGetResult> DefaultGet(ChamCongDefaultGetPost val)
        {
            var res = new ChamCongDefaultGetResult();
            if (val.EmployeeId.HasValue)
            {
                var employeeObj = GetService<IEmployeeService>();
                var employee = await employeeObj.GetByIdAsync(val.EmployeeId);
                res.EmployeeId = employee.Id;
                res.Employee = _mapper.Map<EmployeeBasic>(employee);
                res.CompanyId = employee.CompanyId ?? CompanyId;
            }

            var workEntryTypeObj = GetService<IWorkEntryTypeService>();
            var workEntryType = await workEntryTypeObj.SearchQuery(orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
            if (workEntryType != null)
            {
                res.WorkEntryTypeId = workEntryType.Id;
                res.WorkEntryType = _mapper.Map<WorkEntryTypeBasic>(workEntryType);
            }

            return res;
        }

        public async Task TaoChamCongNgayLe(Guid employeeId, DateTime dateFrom, DateTime dateTo, IEnumerable<AttendanceInterval> attendanceIntervals = null)
        {
            var leaveObj = GetService<IResourceCalendarLeaveService>();
            var empObj = GetService<IEmployeeService>();
            var salaryConfigObj = GetService<IHrSalaryConfigService>();

            var employee = await empObj.SearchQuery(x => x.Id == employeeId).Include(x => x.StructureType)
              .FirstOrDefaultAsync();
            var calendarId = employee.StructureType.DefaultResourceCalendarId;

            // lấy danh sách ngày nghỉ lễ
            var leaves = await leaveObj.SearchQuery(x => x.CalendarId == calendarId.Value && x.DateFrom <= dateTo && x.DateFrom >= dateFrom).ToListAsync();
            if (!leaves.Any()) 
                return;

            // lấy attendanceinterval
            if (attendanceIntervals == null)
            {
                var calendarObj = GetService<IResourceCalendarService>();
                attendanceIntervals = await calendarObj._AttendanceIntervals(calendarId.Value, dateFrom, dateTo);
            }
            var dict = new Dictionary<DateTime, IList<AttendanceInterval>>();
            foreach (var item in attendanceIntervals)
            {
                if (!dict.ContainsKey(item.Start.Date))
                    dict.Add(item.Start.Date, new List<AttendanceInterval>());
                dict[item.Start.Date].Add(item);
            }

            //lấy workentrytype cho ngày lễ
            var workEntryType = (await salaryConfigObj.GetByCompanyId(CompanyId)).DefaultGlobalLeaveType;
            if (workEntryType == null) throw new Exception("Cần thiết lập loại chấm công cho ngày nghỉ lễ trong cấu hình lương");

            //lưu chấm công

            var listChamcongs = await SearchQuery(x => x.EmployeeId == employeeId && x.TimeIn.Value.Date >= dateFrom.Date && x.TimeIn.Value.Date <= dateTo.Date).ToListAsync();

            var chamcongsToAdd = new List<ChamCong>();
            foreach (var leave in leaves)
            {
                var day = leave.DateFrom.Date;
                while (day <= leave.DateTo.Date)
                {
                    if (listChamcongs.Any(x => x.TimeIn.Value.Date == day)) { day = day.AddDays(1); continue; }

                    var intervals = dict.ContainsKey(day) ? dict[day] : new List<AttendanceInterval>();
                    foreach (var interval in intervals)
                    {
                        chamcongsToAdd.Add(new ChamCong()
                        {
                            EmployeeId = employeeId,
                            CompanyId = CompanyId,
                            WorkEntryTypeId = workEntryType.id,
                            Date = day,
                            TimeIn = interval.Start,
                            TimeOut = interval.Stop,
                            Status = "done"
                        });
                    }
                    day = day.AddDays(1);
                }
            }
            await CreateAsync(chamcongsToAdd);
        }

        public async Task TaoChamCongNguyenNgay(TaoChamCongNguyenNgayViewModel val)
        {
            //tạo chấm công làm việc nguyên ngày theo 1 work entry type
            var empObj = GetService<IEmployeeService>();
            var calendarObj = GetService<IResourceCalendarService>();

            var empList = await empObj.SearchQuery(x => x.Active == true).Include(x => x.StructureType).ToListAsync();
            var chamcongsToAdd = new List<ChamCong>();
            foreach (var emp in empList)
            {
                if (emp.StructureType == null) 
                    throw new Exception($"Nhân viên {emp.Name} chưa thiết lập loại mẫu lương!");

                //get chu kì làm việc của ngày đó của nhân viên đó
                var dateFrom = val.Date.AbsoluteBeginOfDate();
                var dateTo = val.Date.AbsoluteEndOfDate();
                var attendanceIntervals = await calendarObj._AttendanceIntervals(emp.StructureType.DefaultResourceCalendarId.Value, dateFrom, dateTo);
                foreach (var interval in attendanceIntervals)
                {
                    chamcongsToAdd.Add(new ChamCong()
                    {
                        CompanyId = CompanyId,
                        EmployeeId = emp.Id,
                        TimeIn = interval.Start,
                        TimeOut = interval.Stop,
                        WorkEntryTypeId = val.WorkEntryTypeId
                    });
                }
            }

            //create list chamcongs
            await CreateAsync(chamcongsToAdd);
        }

        public override ISpecification<ChamCong> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "hr.cham_cong_comp_rule":
                    return new InitialSpecification<ChamCong>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }
    }
}
