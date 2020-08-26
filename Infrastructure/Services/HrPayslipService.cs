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

        public async Task ComputePayslipLine(List<Guid> ids)
        {
            var listPayslip = new List<HrPayslip>();

            var paylist = SearchQuery(x => ids.Contains(x.Id)).ToList();

            foreach (var payslip in paylist)
            {
                var empObj = GetService<IEmployeeService>();
                var structureObj = GetService<IHrPayrollStructureService>();

                payslip.Lines.Clear();
                var workedDayLines = payslip != null && payslip.WorkedDaysLines != null ? payslip.WorkedDaysLines.ToList() : new List<HrPayslipWorkedDays>();
                var emp = await empObj.SearchQuery(x => x.Id == payslip.EmployeeId).FirstOrDefaultAsync();
                var structure = await structureObj.SearchQuery(x => x.Id == payslip.StructId).Include(x => x.Rules).FirstOrDefaultAsync();
                var rules = structure != null && structure.Rules != null ? structure.Rules.ToList() : new List<HrSalaryRule>();
                for (int i = 0; i < rules.Count(); i++)
                {
                    var rule = rules[i];
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
                                //Tinh luong chinh cua Emp = so ngay cong * luong co ban
                                case "LC":

                                    foreach (var item in workedDayLines)
                                    {
                                        line.Amount += item.Amount;
                                    }
                                    line.Total = line.Amount;
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

                                    decimal amount = 0;
                                    foreach (var item in workedDayLines)
                                    {
                                        amount += item.Amount.Value;
                                    }
                                    line.Amount = amount * rule.AmountPercentage / 100;
                                    line.Total = line.Amount;

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
                    payslip.Lines.Add(line);
                    listPayslip.Add(payslip);
                }
            }
            await UpdateAsync(listPayslip);
        }

        public async Task<HrPayslip> GetHrPayslipDisplay(Guid Id)
        {
            var res = await SearchQuery(x => x.Id == Id).Include(x => x.Struct).Include(x => x.Employee).Include(x => x.Lines).Include(x=>x.WorkedDaysLines).FirstOrDefaultAsync();
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
            var soTien1Cong = (employee.Wage.HasValue ? employee.Wage : 0) / (decimal)socongchuan.SoNgayCong;

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

        public async Task ActionConfirm(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var amlObj = GetService<IAccountMoveLineService>();
            var hrPayslips = await SearchQuery(x => ids.Contains(x.Id))
                .Include(x => x.Company)
                .Include(x => x.Lines)
                .ToListAsync();

            foreach (var payslip in hrPayslips)
            {
                if (payslip.State != "process")
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

        public async Task SaveWorkedDayLines(HrPayslipSave val, HrPayslip payslip)
        {
            var ToRemove = new List<HrPayslipWorkedDays>();
            foreach (var wd in payslip.WorkedDaysLines)
            {
                if (!val.ListHrPayslipWorkedDaySave.Any(x => x.Id == wd.Id))
                    ToRemove.Add(wd);
            }

            await GetService<IHrPayslipWorkedDayService>().Remove(ToRemove.Select(x => x.Id).ToList());
            payslip.WorkedDaysLines = payslip.WorkedDaysLines.Except(ToRemove).ToList();

            foreach (var wd in val.ListHrPayslipWorkedDaySave)
            {
                if (wd.Id == Guid.Empty || !wd.Id.HasValue)
                {
                    var r = _mapper.Map<HrPayslipWorkedDays>(wd);
                    payslip.WorkedDaysLines.Add(r);
                }
                else
                {
                    _mapper.Map(wd, payslip.WorkedDaysLines.SingleOrDefault(c => c.Id == wd.Id));
                }
            }
        }
    }

    public class HrPayslipOnChangeEmployeeResult
    {
        public List<HrPayslipWorkedDayDisplay> WorkedDayLines { get; set; } = new List<HrPayslipWorkedDayDisplay>();

        public string Name { get; set; }
    }
}

