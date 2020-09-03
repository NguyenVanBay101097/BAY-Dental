using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public async Task ComputeSheet(IEnumerable<Guid> ids)
        {
            var ruleObj = GetService<IHrSalaryRuleService>();
            var employeeObj = GetService<IEmployeeService>();
            var seqObj = GetService<IIRSequenceService>();

            var payslips = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines)
                .Include(x => x.WorkedDaysLines).ToListAsync();
            foreach (var payslip in payslips)
            {
                var number = !string.IsNullOrEmpty(payslip.Number) ? payslip.Number : await seqObj.NextByCode("salary.slip");
                payslip.Lines.Clear();

                var employee = await employeeObj.GetByIdAsync(payslip.EmployeeId);

                var rules = await ruleObj.SearchQuery(x => x.StructId == payslip.StructId).ToListAsync();

                foreach (var rule in rules)
                {
                    var line = new HrPayslipLine();
                    line.SalaryRuleId = rule.Id;
                    line.Name = rule.Name;
                    line.Code = rule.Code;
                    line.Sequence = rule.Sequence;

                    switch (rule.AmountSelect)
                    {
                        case "code":
                            switch (rule.AmountCodeCompute)
                            {
                                //Lương chính = sum amount worked day lines
                                case "luong_chinh":
                                    line.Amount = payslip.WorkedDaysLines.Sum(x => x.Amount) ?? 0;
                                    break;
                                case "hoa_hong":
                                    break;
                                default:
                                    throw new Exception("Not support");
                            }
                            break;
                        case "fix":
                            line.Amount = rule.AmountFix;
                            break;
                        case "percentage":
                            break;
                        default:
                            break;
                    }

                    payslip.Lines.Add(line);
                }

                payslip.Number = number;
                payslip.State = "verify";
            }

            _ComputeAmount(payslips);
            await UpdateAsync(payslips);
        }

        public async Task<HrPayslipDisplay> GetHrPayslipDisplay(Guid Id)
        {
            var res = await _mapper.ProjectTo<HrPayslipDisplay>(SearchQuery(x => x.Id == Id)).FirstOrDefaultAsync();
            return res;
        }

        public async Task<PagedResult2<HrPayslipBasic>> GetPaged(HrPayslipPaged val)
        {
            var query = SearchQuery();

            if (!string.IsNullOrEmpty(val.Search))
                query = query.Where(x => x.Name.Contains(val.Search) || x.Number.Contains(val.Search));

            if (!string.IsNullOrEmpty(val.State))
                query = query.Where(x => x.State.Contains(val.State));

            if (val.DateFrom.HasValue)
            {
                var dateFrom = val.DateFrom.Value.AbsoluteBeginOfDate();
                query = query.Where(x => x.DateFrom >= dateFrom);
            }

            if (val.DateTo.HasValue)
            {
                var dateTo = val.DateTo.Value.AbsoluteEndOfDate();
                query = query.Where(x => x.DateTo <= dateTo);
            }

            if (val.EmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == val.EmployeeId);
            }

            if (val.PayslipRunId.HasValue)
                query = query.Where(x => x.PayslipRunId == val.PayslipRunId.Value);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await _mapper.ProjectTo<HrPayslipBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();
            var totalItems = await query.CountAsync();
            return new PagedResult2<HrPayslipBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }

        public async Task<HrPayslipOnChangeEmployeeResult> OnChangeEmployee(Guid? employeeId, DateTime? dateFrom, DateTime? dateTo)
        {
            var res = new HrPayslipOnChangeEmployeeResult();

            if (!employeeId.HasValue || !dateFrom.HasValue || !dateTo.HasValue)
                return res;

            var empObj = GetService<IEmployeeService>();
            var attObj = GetService<IResourceCalendarAttendanceService>();
            var ccObj = GetService<IChamCongService>();
            var calendarObj = GetService<IResourceCalendarService>();
            var workEntryTypeObj = GetService<IWorkEntryTypeService>();

            var employee = await empObj.SearchQuery(x => x.Id == employeeId).Include(x => x.StructureType)
                .FirstOrDefaultAsync();

            if (employee.StructureType == null)
                return res;

            var structureType = employee.StructureType;
            res.StructureType = _mapper.Map<HrPayrollStructureTypeSimple>(structureType);
            res.StructureTypeId = structureType.Id;

            res.Name = $"Phiếu lương tháng {dateFrom.Value.ToString("M yyyy", new CultureInfo("vi-VN")).ToLower()} {employee.Name}";

            var calendarId = structureType.DefaultResourceCalendarId;

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
            var soTien1Cong = (employee.Wage.HasValue ? employee.Wage : 0) / (decimal)socongchuan.SoNgayCong;

            //tạo ra output
            var work_entry_type_ids = work_entry_type_dict.Keys.ToArray();
            var work_entry_types = await workEntryTypeObj.SearchQuery(x => work_entry_type_ids.Contains(x.Id)).ToListAsync();
            var work_entry_type_dict_2 = work_entry_types.ToDictionary(x => x.Id, x => x);

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

                res.WorkedDayLines.Add(new HrPayslipWorkedDayDisplay
                {
                    NumberOfDays = (decimal)item.Value.SoNgayCong,
                    NumberOfHours = (decimal)item.Value.SoGioCong,
                    WorkEntryTypeId = item.Key,
                    Name = work_entry_type.Name,
                    Amount = amount
                });
            }

            return res;
        }

        public async Task<HrPayslip> CreatePayslip(HrPayslipSave val)
        {
            var payslip = _mapper.Map<HrPayslip>(val);
            payslip.CompanyId = CompanyId;
            SaveWorkedDayLines(val, payslip);

            return await CreateAsync(payslip);
        }

        public async Task UpdatePayslip(Guid id, HrPayslipSave val)
        {
            var payslip = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            if (payslip == null)
                throw new Exception("payslip Not Found");

            payslip = _mapper.Map(val, payslip);
            SaveWorkedDayLines(val, payslip);

            await UpdateAsync(payslip);
        }

        private void SaveWorkedDayLines(HrPayslipSave val, HrPayslip payslip)
        {
            var toRemove = new List<HrPayslipWorkedDays>();
            foreach (var wd in payslip.WorkedDaysLines)
            {
                if (!val.WorkedDaysLines.Any(x => x.Id == wd.Id))
                    toRemove.Add(wd);
            }

            foreach (var item in toRemove)
                payslip.WorkedDaysLines.Remove(item);

            var sequence = 0;
            foreach (var wd in val.WorkedDaysLines)
            {
                if (wd.Id == Guid.Empty)
                {
                    var r = _mapper.Map<HrPayslipWorkedDays>(wd);
                    r.Sequence = sequence++;
                    payslip.WorkedDaysLines.Add(r);
                }
                else
                {
                    var line = payslip.WorkedDaysLines.FirstOrDefault(c => c.Id == wd.Id);
                    if (line != null)
                    {
                        _mapper.Map(wd, line);
                        line.Sequence = sequence++;
                    }
                }
            }
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var hrPayslips = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Company)
                .Include(x => x.Lines)
                .ToListAsync();

            foreach (var payslip in hrPayslips)
            {
                if (payslip.State != "verify")
                    throw new Exception("Chỉ những phiếu lương chờ xác nhận được vào sổ.");

                var move = await _PreparePayslipMovesAsync(payslip);
                amlObj.PrepareLines(move.Lines);
                await moveObj.CreateMoves(new List<AccountMove> { move });
                await moveObj.ActionPost(new List<AccountMove> { move });
                payslip.State = "done";
                payslip.AccountMoveId = move.Id;
            }

            await UpdateAsync(hrPayslips);
        }

        private async Task<AccountMove> _PreparePayslipMovesAsync(HrPayslip payslip)
        {
            var accountJournalObj = GetService<IAccountJournalService>();

            var accountJournal = await accountJournalObj.GetJournalByTypeAndCompany("payroll", payslip.CompanyId);
            if (accountJournal == null)
                accountJournal = await InsertAccountJournalIfNotExists();


            var move = new AccountMove
            {
                JournalId = accountJournal.Id,
                Journal = accountJournal,
                CompanyId = payslip.CompanyId,
            };

            var lines = new List<AccountMoveLine>();
            foreach (var line in payslip.Lines)
            {
                var balance = line.Amount.Value;
                var items = new List<AccountMoveLine>()
                {
                    new AccountMoveLine
                    {
                        Name =  line.Name,
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        AccountId = accountJournal.DefaultDebitAccount.Id,
                        Account = accountJournal.DefaultDebitAccount,
                        Move = move,
                    },
                    new AccountMoveLine
                    {
                        Name = line.Name,
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        AccountId = accountJournal.DefaultCreditAccount.Id,
                        Account = accountJournal.DefaultCreditAccount,
                        Move = move,
                    },
                };

                lines.AddRange(items);

            }
            move.Lines = lines;
            return move;
        }

        public async Task<AccountJournal> InsertAccountJournalIfNotExists()
        {
            var irModelDataObj = GetService<IIRModelDataService>();
            var accountObj = GetService<IAccountAccountService>();
            var accountJournalObj = GetService<IAccountJournalService>();

            var currentLiabilities = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
            var acc334 = new AccountAccount
            {
                Name = "Phải trả người lao động",
                Code = "334",
                InternalType = currentLiabilities.Type,
                UserTypeId = currentLiabilities.Id,
                CompanyId = CompanyId,
            };

            var expensesType = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_expenses");
            var acc642 = new AccountAccount
            {
                Name = "Chi phí quản lý doanh nghiệp",
                Code = "642",
                InternalType = expensesType.Type,
                UserTypeId = expensesType.Id,
                CompanyId = CompanyId,
            };

            await accountObj.CreateAsync(new List<AccountAccount>() { acc334, acc642 });

            var salaryJournal = new AccountJournal
            {
                Name = "Nhật ký lương",
                Type = "payroll",
                UpdatePosted = true,
                Code = "SALARY",
                DefaultDebitAccountId = acc642.Id,
                DefaultCreditAccountId = acc334.Id,
                CompanyId = CompanyId,
            };

            await accountJournalObj.CreateAsync(salaryJournal);

            return salaryJournal;
        }

        public HrPayslipDefaultGetResult DefaultGet(HrPayslipDefaultGet val)
        {
            var now = DateTime.Now;
            var tmp = now.AddMonths(1);
            var tmp2 = new DateTime(tmp.Year, tmp.Month, 1);

            var res = new HrPayslipDefaultGetResult()
            {
                CompanyId = CompanyId,
                DateFrom = new DateTime(now.Year, now.Month, 1),
                DateTo = tmp2.AddDays(-1).Date,
            };

            return res;
        }

        public async Task Unlink(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).ToListAsync();
            var states = new string[] { "draft", "cancel" };
            if (self.Any(x => !states.Contains(x.State)))
                throw new Exception("Bạn chỉ có thể xóa phiếu lương ở trạng thái nháp hoặc hủy bỏ");
            await DeleteAsync(self);
        }

        public async Task<IEnumerable<HrPayslipWorkedDayBasic>> GetWorkedDaysLines(Guid id)
        {
            var workedDaysObj = GetService<IHrPayslipWorkedDayService>();
            var query = workedDaysObj.SearchQuery(x => x.PayslipId == id, orderBy: x => x.OrderBy(s => s.Sequence));
            var res = await _mapper.ProjectTo<HrPayslipWorkedDayBasic>(query).ToListAsync();
            return res;
        }

        public async Task<IEnumerable<HrPayslipLineBasic>> GetLines(Guid id)
        {
            var lineObj = GetService<IHrPayslipLineService>();
            var query = lineObj.SearchQuery(x => x.SlipId == id, orderBy: x => x.OrderBy(s => s.Sequence));
            var res = await _mapper.ProjectTo<HrPayslipLineBasic>(query).ToListAsync();
            return res;
        }

        public async Task _ComputeAmount(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Lines).ToListAsync();
            _ComputeAmount(self);
            await UpdateAsync(self);
        }

        public void _ComputeAmount(IEnumerable<HrPayslip> self)
        {
            foreach (var payslip in self)
                payslip.TotalAmount = payslip.Lines.Sum(x => x.Amount);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var self = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.AccountMove).ToListAsync();
            var moveObj = GetService<IAccountMoveService>();
            var move_ids = self.Where(x => x.AccountMove != null && x.AccountMove.State == "posted").Select(x => x.AccountMove.Id);
            await moveObj.ButtonCancel(move_ids);
            await moveObj.Unlink(move_ids);

            //if (self.Any(x => x.State == "done"))
            //    throw new Exception("Không thể hủy phiếu lương đã hoàn thành");

            foreach (var payslip in self)
                payslip.State = "draft";
            await UpdateAsync(self);
        }
    }

    public class HrPayslipOnChangeEmployeeResult
    {
        public List<HrPayslipWorkedDayDisplay> WorkedDayLines { get; set; } = new List<HrPayslipWorkedDayDisplay>();

        public string Name { get; set; }

        public Guid? StructureTypeId { get; set; }
        public HrPayrollStructureTypeSimple StructureType { get; set; }
    }

    public class HrPayslipDefaultGetResult
    {
        public Guid CompanyId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }
    }
}

