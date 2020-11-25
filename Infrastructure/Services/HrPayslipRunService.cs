using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class HrPayslipRunService : BaseService<HrPayslipRun>, IHrPayslipRunService
    {
        private readonly IMapper _mapper;

        public HrPayslipRunService(IAsyncRepository<HrPayslipRun> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<PagedResult2<HrPayslipRunBasic>> GetPagedResultAsync(HrPayslipRunPaged val)
        {
            ISpecification<HrPayslipRun> spec = new InitialSpecification<HrPayslipRun>(x => true);
            if (!string.IsNullOrEmpty(val.Search))
                spec = spec.And(new InitialSpecification<HrPayslipRun>(x => x.Name.Contains(val.Search)));

            var query = SearchQuery(spec.AsExpression(), orderBy: x => x.OrderByDescending(s => s.DateCreated));
            var items = await _mapper.ProjectTo<HrPayslipRunBasic>(query.Skip(val.Offset).Take(val.Limit)).ToListAsync();

            var totalItems = await query.CountAsync();

            return new PagedResult2<HrPayslipRunBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = items
            };
        }


        public async Task<HrPayslipRunDisplay> GetHrPayslipRunForDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<HrPayslipRunDisplay>(SearchQuery(x => x.Id == id)
                .Include("Slips.SalaryPayment").Include(x => x.Slips).ThenInclude(x => x.Employee))
                .FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Đợt lương không tồn tại");
            res.IsExistSalaryPayment = res.Slips.Any(x=> x.SalaryPayment != null);
            // get user
            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            return res;
        }

        public async Task<HrPayslipRun> CreatePayslipRun(HrPayslipRunSave val)
        {
            var isExist = await SearchQuery().AnyAsync(x => x.Date.Value.Month == val.Date.Value.Month && x.Date.Value.Year == val.Date.Value.Year);
            if (isExist == true)
            {
                throw new Exception("đã tồn tại bảng lương của tháng " + val.Date.Value.Month);
            }
            var payslipRun = _mapper.Map<HrPayslipRun>(val);

            return await CreateAsync(payslipRun);
        }

        public async Task UpdatePayslipRun(Guid id, HrPayslipRunSave val)
        {
            var paySlipRun = await SearchQuery(x => x.Id == id).Include(x => x.Slips).Include(x => x.Company).FirstOrDefaultAsync();
            if (paySlipRun == null)
                throw new Exception("Đợt lương không tồn tại");

            paySlipRun = _mapper.Map(val, paySlipRun);
            SaveSlips(val, paySlipRun);
            await UpdateAsync(paySlipRun);
            await ComputeSalaryByRunId(id);
        }

        public void SaveSlips(HrPayslipRunSave val, HrPayslipRun run)
        {
            var toRemove = new List<HrPayslip>();
            foreach (var slip in run.Slips)
            {
                if (!val.Slips.Any(x => x.Id == slip.Id))
                    toRemove.Add(slip);
            }

            foreach (var item in toRemove)
                run.Slips.Remove(item);

            foreach (var slip in val.Slips)
            {
                if (slip.Id == Guid.Empty)
                {
                    var r = _mapper.Map<HrPayslip>(slip);
                    run.Slips.Add(r);
                }
                else
                {
                    var sl = run.Slips.FirstOrDefault(c => c.Id == slip.Id);
                    if (sl != null)
                    {
                        _mapper.Map(slip, sl);
                    }
                }
            }
        }

        public async Task ActionConfirm(Guid id)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var moveObj = GetService<IAccountMoveService>();
            var amlObj = GetService<IAccountMoveLineService>();

            var paysliprun = await SearchQuery(x => x.Id == id).Include(x => x.Slips).ThenInclude(x => x.Employee)
                .ThenInclude(x=> x.Partner).FirstOrDefaultAsync();
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");
            // ghi sổ
            var move = await PreparePayslipMove(paysliprun);
            amlObj.PrepareLines(move.Lines);

            await moveObj.CreateMoves(new List<AccountMove> { move });
            await moveObj.ActionPost(new List<AccountMove> { move });
            // update bảng lương
            paysliprun.State = "done";
            paysliprun.MoveId = move.Id;
            await UpdateAsync(paysliprun);
        }

        private async Task<AccountMove> PreparePayslipMove(HrPayslipRun slipRun)
        {
            if (slipRun.State != "confirm")
                throw new Exception("chỉ có ở trạng thái xác nhận mới được ghi vào sổ");

            var accountJournalObj = GetService<IAccountJournalService>();
            var slipObj = GetService<IHrPayslipService>();
            var accountObj = GetService<IAccountAccountService>();
            var irModelDataObj = GetService<IIRModelDataService>();

            var accountJournal = await accountJournalObj.GetJournalByTypeAndCompany("payroll", slipRun.CompanyId);
            if (accountJournal == null)
                accountJournal = await slipObj.InsertAccountJournalIfNotExists();
            var acc334 = accountObj.SearchQuery(x=> x.CompanyId == CompanyId && x.Code == "334").FirstOrDefault();
            if (acc334 != null)
            {
                var currentLiabilities = await irModelDataObj.GetRef < AccountAccountType >("account.data_account_type_current_liabilities");
                acc334 = new AccountAccount
                {
                    Name = "Phải trả người lao động",
                    Code = "334",
                    InternalType = currentLiabilities.Type,
                    UserTypeId = currentLiabilities.Id,
                    CompanyId = CompanyId,
                };

                await accountObj.CreateAsync(acc334);
            }


            //tạo 1 move cho toàn bộ bảng lương
            var move = new AccountMove
            {
                Date = DateTime.Now,
                JournalId = accountJournal.Id,
                Journal = accountJournal,
                CompanyId = slipRun.CompanyId,
            };
            // tạo moveline cho từng phiếu lương, 1 phiếu 2 line
            var lines = new List<AccountMoveLine>();
            foreach (var slip in slipRun.Slips)
            {
                var balance = slip.TotalSalary.GetValueOrDefault();
                var items = new List<AccountMoveLine>()
                {
                    new AccountMoveLine
                    {
                        Name =  "Lương tháng "+move.Date.ToString("MM/yyyy"),
                        Debit = balance > 0 ? balance : 0,
                        Credit = balance < 0 ? -balance : 0,
                        AccountId = acc334.Id,
                        Account = acc334,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                    },
                    new AccountMoveLine
                    {
                        Name = "Lương tháng "+move.Date.ToString("MM/yyyy"),
                        Debit = balance < 0 ? -balance : 0,
                        Credit = balance > 0 ? balance : 0,
                        AccountId = accountJournal.DefaultCreditAccount.Id,
                        Account = accountJournal.DefaultCreditAccount,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                    },
                };
                lines.AddRange(items);
            }
            move.Lines = lines;

            return move;
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id) && x.State == "confirm").Include(x => x.Slips).ToListAsync();

            foreach (var run in payslipruns)
            {
                foreach (var slip in run.Slips)
                {
                    if (slip.State == "done")
                        continue;

                    await payslipObj.ActionDone(new List<Guid> { slip.Id });
                }

                run.State = "done";
            }

            await UpdateAsync(payslipruns);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var moveObj = GetService<IAccountMoveService>();
            var moveLineObj = GetService<IAccountMoveLineService>();
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id)).Include("Slips.SalaryPayment")
                .Include(x => x.Move).ThenInclude(x => x.Lines).ToListAsync();

            var listMove = new List<AccountMove>();
            var listMoveLine = new List<AccountMoveLine>();
            foreach (var run in payslipruns)
            {
                if (run.Slips.Any(x => x.SalaryPayment != null)) throw new Exception("đã có phiếu lương được chi lương nên không thể hủy!");
                listMove.Add(run.Move);
                listMoveLine.AddRange(run.Move.Lines);
                run.State = "confirm";
            }
            await moveLineObj.DeleteAsync(listMoveLine);
            await moveObj.DeleteAsync(listMove);

            await UpdateAsync(payslipruns);
        }

        public override ISpecification<HrPayslipRun> RuleDomainGet(IRRule rule)
        {
            var userObj = GetService<IUserService>();
            var companyIds = userObj.GetListCompanyIdsAllowCurrentUser();
            switch (rule.Code)
            {
                case "hr.payslip_run_comp_rule":
                    return new InitialSpecification<HrPayslipRun>(x => companyIds.Contains(x.CompanyId));
                default:
                    return null;
            }
        }

        public async Task<HrPayslipRunDisplay> CreatePayslipByRunId(Guid id)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var employeeObj = GetService<IEmployeeService>();
            var ccObj = GetService<IChamCongService>();
            var commissionObj = GetService<ICommissionSettlementService>();
            var advanceObj = GetService<ISalaryPaymentService>();

            //get all resource
            var emps = await employeeObj.SearchQuery(x => x.Active == true).ToListAsync();
            var empIds = emps.Select(x => x.Id).ToList();
            var paysliprun = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var allChamcongs = await ccObj.SearchQuery(c => empIds.Any(x => x == c.EmployeeId)
            && c.Date.Value.Month == paysliprun.Date.Value.Month
            && c.Date.Value.Year == paysliprun.Date.Value.Year).ToListAsync();
            var firstDayOfMonth = new DateTime(paysliprun.Date.Value.Year, paysliprun.Date.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var commissions = await commissionObj.GetReport(new CommissionSettlementReport() { CompanyId = paysliprun.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth });
            var Alladvances = await advanceObj.SearchQuery(c => empIds.Contains(c.EmployeeId.Value) 
            && c.Type == "advance" && c.State == "done"
            && c.Date.Month == paysliprun.Date.Value.Month
            && c.Date.Year == paysliprun.Date.Value.Year).ToListAsync();

            //validate
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");

            //tạo phiếu lương từ list emps
            var payslips = new List<HrPayslip>();
            foreach (var emp in emps)
            {
                var payslip = new HrPayslip();
                payslip.CompanyId = paysliprun.CompanyId;
                payslip.EmployeeId = emp.Id;
                payslip.Employee = emp;
                payslip.PayslipRunId = paysliprun.Id;
                payslip.Name = $"Phiếu lương tháng {paysliprun.Date.Value.ToString("M yyyy", new CultureInfo("vi-VN")).ToLower()} {emp.Name}";

                //tính toán các khoản lương
                var chamCongs = allChamcongs.Where(x => x.EmployeeId == emp.Id);
                var commission = commissions.FirstOrDefault(x => x.EmployeeId == emp.Id);
                var advance = Alladvances.Where(x => x.EmployeeId == emp.Id);

                await ComputeSalary(payslip, chamCongs, commission, advance, paysliprun.Date.Value);

                payslips.Add(payslip);
            }

            // lưu phiếu lương, bảng lương
            await payslipObj.CreateAsync(payslips);

            paysliprun.State = "confirm";
            await UpdateAsync(paysliprun);
            return await this.GetHrPayslipRunForDisplay(paysliprun.Id);
        }

        public async Task ComputeSalary(HrPayslip payslip, IEnumerable<ChamCong> chamCongs = null, CommissionSettlementReportOutput commission = null, IEnumerable<SalaryPayment> advances = null, DateTime? date = null)
        {
            var empObj = GetService<IEmployeeService>();
            var ccObj = GetService<IChamCongService>();
            var commissionObj = GetService<ICommissionSettlementService>();
            var advanceObj = GetService<ISalaryPaymentService>();
            date = date.HasValue ? date : DateTime.Now;

            var emp = payslip.Employee;
            if (emp == null)
            {
                emp = await empObj.GetByIdAsync(payslip.EmployeeId);
            }

            if (chamCongs == null)
            {
                chamCongs = await ccObj.SearchQuery(c => c.EmployeeId == emp.Id
            && c.Date.Value.Month == date.Value.Month
            && c.Date.Value.Year == date.Value.Year).ToListAsync();
            }
            if (commission == null)
            {
                var firstDayOfMonth = new DateTime(date.Value.Year, date.Value.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var allCommission = await commissionObj.GetReport(new CommissionSettlementReport() { CompanyId = payslip.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth, EmployeeId = emp.Id });
                commission = allCommission.FirstOrDefault(x => x.EmployeeId == emp.Id);
            }
            if (advances == null)
            {
                advances = await advanceObj.SearchQuery(c => c.EmployeeId == emp.Id
             && c.Type == "advance" && c.State == "done" && c.Date.Month == date.Value.Month
            && c.Date.Year == date.Value.Year).ToListAsync();
            }

            payslip.DaySalary = (emp.Wage.GetValueOrDefault() / (DateTime.DaysInMonth(date.Value.Year, date.Value.Month) - emp.LeavePerMonth.GetValueOrDefault()));

            payslip.WorkedDay = Math.Min(chamCongs.Where(x => x.Type == "work").Count() + (chamCongs.Where(x=> x.Type == "halfaday").Count()/2)
                , DateTime.DaysInMonth(date.Value.Year, date.Value.Month) - emp.LeavePerMonth.GetValueOrDefault());

            payslip.ActualLeavePerMonth = DateTime.DaysInMonth(date.Value.Year, date.Value.Month) - payslip.WorkedDay;
            payslip.LeavePerMonthUnpaid = DateTime.DaysInMonth(date.Value.Year, date.Value.Month) - payslip.WorkedDay - emp.LeavePerMonth.GetValueOrDefault();

            payslip.OverTimeDay = Math.Max((emp.LeavePerMonth.GetValueOrDefault() - chamCongs.Where(x => x.Type == "off").Count()), 0);
            payslip.TotalBasicSalary = Math.Round(payslip.WorkedDay.GetValueOrDefault() * payslip.DaySalary.GetValueOrDefault(), 0);

            payslip.OverTimeHour = chamCongs.Where(x => x.OverTime == true).Sum(x => x.OverTimeHour.GetValueOrDefault());
            payslip.OverTimeHourSalary = emp.RegularHour.GetValueOrDefault() == 0 ? 0 : Math.Round(((payslip.DaySalary.GetValueOrDefault() / emp.RegularHour.GetValueOrDefault()) * (emp.OvertimeRate.GetValueOrDefault() / 100) * payslip.OverTimeHour.GetValueOrDefault()), 0);

            payslip.OverTimeDaySalary = Math.Round((payslip.OverTimeDay.GetValueOrDefault() * payslip.DaySalary.GetValueOrDefault() * (emp.RestDayRate.GetValueOrDefault() / 100)), 0);
            payslip.Allowance = emp.Allowance.GetValueOrDefault();

            payslip.CommissionSalary = commission == null ? 0 : Math.Round(commission.Amount.GetValueOrDefault(), 0);
            payslip.AmercementMoney = 0;

            payslip.TotalSalary = payslip.TotalBasicSalary + payslip.OverTimeHourSalary + payslip.OverTimeDaySalary + payslip.Allowance
               + payslip.OtherAllowance.GetValueOrDefault() + payslip.RewardSalary.GetValueOrDefault() + payslip.HolidayAllowance.GetValueOrDefault()
               + payslip.CommissionSalary - payslip.AmercementMoney.Value;
            payslip.AdvancePayment = advances.Sum(x => x.Amount);
            payslip.NetSalary = payslip.TotalSalary - payslip.AdvancePayment;

        }

        public async Task ComputeSalaryByRunId(Guid id)
        {
            var paysliprun = await SearchQuery(x => x.Id == id).Include(x => x.Slips).ThenInclude(x => x.Employee).FirstOrDefaultAsync();

            //validate
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");
            foreach (var item in paysliprun.Slips)
            {
                await ComputeSalary(item, null, null, null, paysliprun.Date);
            }
            await UpdateAsync(paysliprun);
        }

        public async Task<HrPayslipRunDisplay> CheckExist(DateTime date)
        {
            var run = await this.SearchQuery(x => x.Date.Value.Month == date.Month && x.Date.Value.Year == date.Year).FirstOrDefaultAsync();
            if(run == null) { return null; }
            return await this.GetHrPayslipRunForDisplay(run.Id);
        }
    }

    public class PaySlipRunConfirmViewModel
    {
        public Guid? PayslipRunId { get; set; }

        public Guid? StructureId { get; set; }

        public IEnumerable<Guid> EmpIds { get; set; } = new List<Guid>();
    }
}
