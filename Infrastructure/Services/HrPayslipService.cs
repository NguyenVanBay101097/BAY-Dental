using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class HrPayslipService : BaseService<HrPayslip>, IHrPayslipService
    {

        private readonly IMapper _mapper;
        public HrPayslipService(IAsyncRepository<HrPayslip> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
           : base(repository, httpContextAccessor)
        {
            _mapper = mapper;

        }

        public async Task<IEnumerable<HrPayslipLine>> ComputePayslipLine(Guid? EmployeeId, IEnumerable<HrPayslipWorkedDaySave> hrPayslipWorkedDaySaves)
        {
            var res = new List<HrPayslipLine>();
            var empObj = GetService<IEmployeeService>();
            var structureObj = GetService<IHrPayrollStructureService>();
            var emp = await empObj.SearchQuery(x => x.Id == EmployeeId.Value).FirstOrDefaultAsync();
            var structure = await structureObj.SearchQuery(x => x.TypeId == emp.StructureTypeId).Include(x => x.Rules).FirstOrDefaultAsync();
            var rules = structure != null && structure.Rules != null ? structure.Rules.ToList() : new List<HrSalaryRule>();
            for (int i = 0; i < rules.Count(); i++)
            {
                var rule = rules[i];
                var line = new HrPayslipLine();
                line.SalaryRuleId = rule.Id;
                line.Name = rule.Name;
                line.Code = rule.Code;
                line.Sequence = i;
                switch (rule.AmountSelect)
                {
                    case "code":
                        switch (rule.AmountCodeCompute)
                        {
                            //Tinh luong chinh cua Emp = so ngay cong * luong co ban
                            case "LC":
                                if (hrPayslipWorkedDaySaves != null && hrPayslipWorkedDaySaves.Count() > 0)
                                {
                                    foreach (var item in hrPayslipWorkedDaySaves)
                                    {
                                        line.Amount += item.Amount;
                                    }
                                    line.Total = line.Amount;
                                }
                                break;
                            //Hoa hong get tu service hoa hong
                            case "HH":

                                break;

                            default:
                                break;
                        }
                        break;

                    case "fixamount":
                        line.Amount = rule.AmountFix;
                        line.Total = rule.AmountFix;
                        break;

                    case "percent":

                        switch (rule.AmountPercentageBase)
                        {
                            //% dua vao luong chinh
                            case "LC":
                                if (hrPayslipWorkedDaySaves != null && hrPayslipWorkedDaySaves.Count() > 0)
                                {
                                    decimal amount = 0;
                                    foreach (var item in hrPayslipWorkedDaySaves)
                                    {
                                        amount += item.Amount.Value;
                                    }
                                    line.Amount = amount * rule.AmountPercentage / 100;
                                    line.Total = line.Amount;
                                }
                                break;
                            //% dua vao hoa hong
                            case "HH":

                                break;

                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
                res.Add(line);
            }
            return res;
        }

        public async Task<HrPayslip> GetHrPayslipDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.Struct).Include(x => x.Employee).Include(x => x.Lines).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayslipDisplay>> GetPaged(HrPayslipPaged val)
        {
            var query = SearchQuery();
            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.Employee.Name.Contains(val.Search));
            }

            if (!string.IsNullOrEmpty(val.State))
            {
                query = query.Where(x => x.State.Contains(val.State));
            }
            if (val.DateFrom.HasValue && val.DateTo.HasValue)
            {
                query = query.Where(x => x.DateFrom >= val.DateFrom && x.DateTo <= val.DateTo);
            }
            query = query.Include(x => x.Struct).Include(x => x.Employee).OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayslipDisplay>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<HrPayslipDisplay>>(items)
            };
        }

        public async Task<HrPayslipOnChangeEmployeeResult> OnChangeEmployee(Guid? employeeId, DateTime? dateFrom, DateTime? dateTo)
        {
            if (!employeeId.HasValue || !dateFrom.HasValue || !dateTo.HasValue)
            {
                return new HrPayslipOnChangeEmployeeResult();
            }

            var empObj = GetService<IEmployeeService>();
            var attObj = GetService<IResourceCalendarAttendanceService>();
            var ccObj = GetService<IChamCongService>();
            var calendarObj = GetService<IResourceCalendarService>();
            var workEntryTypeObj = GetService<IWorkEntryTypeService>();

            var employee = await empObj.SearchQuery(x => x.Id == employeeId).Include(x => x.StructureType)
                .FirstOrDefaultAsync();

            var calendarId = employee.StructureType.DefaultResourceCalendarId;

            //list chu kỳ làm việc
            var attendanceIntervals = await calendarObj._AttendanceIntervals(calendarId.Value, dateFrom.Value, dateTo.Value);

            //tìm những chấm công theo nhân viên từ ngày đến ngày của tháng
            var listChamcongs = await ccObj.SearchQuery(x => x.EmployeeId == employeeId && x.TimeIn.Value.Date >= dateFrom.Value.Date && x.TimeIn.Value.Date <= dateTo.Value.Date).ToListAsync();

            //gom lại theo từng work entry type
            var work_entry_type_dict = new Dictionary<Guid, ChamCongTinhCong>();

            var calendar = await calendarObj.GetByIdAsync(calendarId);

            foreach (var chamCong in listChamcongs)
            {
                if (!work_entry_type_dict.ContainsKey(chamCong.WorkEntryTypeId.Value))
                    work_entry_type_dict.Add(chamCong.WorkEntryTypeId.Value, new ChamCongTinhCong());

                var tinhCongResult = ccObj.TinhSoCongGioCong(chamCong, attendanceIntervals, calendar.HoursPerDay.HasValue ? calendar.HoursPerDay.Value : 8);

                work_entry_type_dict[chamCong.WorkEntryTypeId.Value].SoGioCong += tinhCongResult.SoGioCong;
                work_entry_type_dict[chamCong.WorkEntryTypeId.Value].SoNgayCong += tinhCongResult.SoNgayCong;
            }
            // input cho tính tiền từng workedday line
            var socongchuan = await calendarObj.TinhSoCongChuan(calendarId.Value, dateFrom.Value, dateTo.Value, attendanceIntervals);
            var structureType = employee.StructureType;
            var soTien1Cong = (employee.Wage.HasValue ? employee.Wage: 0) / (decimal)socongchuan.SoNgayCong;

            //tạo ra output
            var work_entry_type_ids = work_entry_type_dict.Keys.ToArray();
            var work_entry_types = await workEntryTypeObj.SearchQuery(x => work_entry_type_ids.Contains(x.Id)).ToListAsync();
            var work_entry_type_dict_2 = work_entry_types.ToDictionary(x => x.Id, x => x);

            var result = new HrPayslipOnChangeEmployeeResult();
            foreach (var item in work_entry_type_dict)
            {
                var work_entry_type = work_entry_type_dict_2[item.Key];
                decimal amount = 0;
                if (structureType.WageType == "monthly")
                {
                    amount = (decimal)item.Value.SoNgayCong * soTien1Cong.Value;
                }
                else if (structureType.WageType == "hourly")
                {
                    amount = (decimal)item.Value.SoGioCong * employee.HourlyWage.Value;
                }

                result.WorkedDayLines.Add(new HrPayslipWorkedDayDisplay
                {
                    NumberOfDays = (decimal)item.Value.SoNgayCong,
                    NumberOfHours = (decimal)item.Value.SoGioCong,
                    WorkEntryTypeId = item.Key,
                    Name = work_entry_type.Name,
                    Amount = amount
                });
            }

            return result;
        }

        public void ComputeSheet(IEnumerable<Guid> ids)
        {

        }
    }

    public class HrPayslipOnChangeEmployeeResult
    {
        public List<HrPayslipWorkedDayDisplay> WorkedDayLines { get; set; } = new List<HrPayslipWorkedDayDisplay>();

        public string Name { get; set; }
    }
}
