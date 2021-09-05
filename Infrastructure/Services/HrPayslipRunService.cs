﻿using ApplicationCore.Entities;
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
            ISpecification<HrPayslipRun> spec = new InitialSpecification<HrPayslipRun>(x => x.CompanyId == CompanyId);
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
            //bị lỗi là các đợt logic cũ có  nhân viên chi nhánh khác
            var res = await _mapper.ProjectTo<HrPayslipRunDisplay>(SearchQuery(x => x.Id == id && x.CompanyId == CompanyId)
                .Include(x => x.Slips).ThenInclude(x => x.Employee)
                .Include(x => x.Slips).ThenInclude(x => x.SalaryPayment))
                .FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Đợt lương không tồn tại");
            res.IsExistSalaryPayment = res.Slips.Any(x => x.SalaryPayment != null);
            // get user
            var userManager = (UserManager<ApplicationUser>)_httpContextAccessor.HttpContext.RequestServices.GetService(typeof(UserManager<ApplicationUser>));
            var user = await userManager.FindByIdAsync(UserId);
            res.User = _mapper.Map<ApplicationUserSimple>(user);
            return res;
        }


        public async Task<HrPayslipRunPrintVm> GetHrPayslipRunForPrint(Guid id)
        {
            var userObj = GetService<IUserService>();
            var slipObj = GetService<IHrPayslipService>();
            var res = await SearchQuery(x => x.Id == id && x.CompanyId == CompanyId).Select(x => new HrPayslipRunPrintVm
            {
                Id = x.Id,
                Name = x.Name,
                CompanyId = x.CompanyId,
                Company = new CompanyPrintVM
                {
                    Name = x.Company.Name,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Logo = x.Company.Logo,
                    PartnerCityName = x.Company.Partner.CityName,
                    PartnerDistrictName = x.Company.Partner.DistrictName,
                    PartnerWardName = x.Company.Partner.WardName,
                    PartnerStreet = x.Company.Partner.Street,
                },
                Date = x.Date,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd,
                State = x.State,
                CreatedById = x.CreatedById
            }).FirstOrDefaultAsync();

            if (res == null)
                throw new NullReferenceException("Đợt lương không tồn tại");

            var user = await userObj.GetByIdAsync(res.CreatedById);
            res.UserName = user.Name;
            res.IsExistSalaryPayment = res.Slips.Any(x => x.SalaryPayment != null);
            res.Slips = _mapper.Map<IEnumerable<HrPayslipDisplay>>(await slipObj.SearchQuery(x => x.PayslipRunId.HasValue ? (x.PayslipRunId.Value == res.Id) : x.PayslipRunId.Value == res.Id).Include(x => x.Employee).Include(x => x.SalaryPayment).ToListAsync());
            return res;
        }

        public async Task<HrPayslipRun> CreatePayslipRun(HrPayslipRunSave val)
        {

            var ccObj = GetService<IChamCongService>();

            var isExist = await SearchQuery(x=> x.CompanyId == CompanyId).AnyAsync(x => x.Date.Value.Month == val.Date.Value.Month && x.Date.Value.Year == val.Date.Value.Year);
            if (isExist == true)
            {
                throw new Exception("Đã tồn tại bảng lương của tháng " + val.Date.Value.Month);
            }
            var payslipRun = _mapper.Map<HrPayslipRun>(val);

            return await CreateAsync(payslipRun);
        }

        public async Task UpdatePayslipRun(Guid id, HrPayslipRunSave val)
        {
            var paySlipRun = await SearchQuery(x => x.Id == id && x.CompanyId == CompanyId).Include(x => x.Slips).Include(x => x.Company).FirstOrDefaultAsync();
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

            var paysliprun = await SearchQuery(x => x.Id == id && x.CompanyId == CompanyId).Include(x => x.Slips).ThenInclude(x => x.Employee)
                .ThenInclude(x => x.Partner).FirstOrDefaultAsync();
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");

            DateTime dayone = new DateTime(paysliprun.Date.Value.AddMonths(1).Year, paysliprun.Date.Value.AddMonths(1).Month, 1);
            if (DateTime.Now < dayone)
                throw new Exception(@$"Bảng lương tháng {paysliprun.Date.Value.ToString("MM/yyyy")} chỉ có thể xác nhận từ ngày {dayone.ToString("dd/MM/yyyy")}");

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
                throw new Exception("Chỉ có ở trạng thái xác nhận mới được ghi vào sổ");

            var accountJournalObj = GetService<IAccountJournalService>();
            var slipObj = GetService<IHrPayslipService>();
            var accountObj = GetService<IAccountAccountService>();
            var irModelDataObj = GetService<IIRModelDataService>();

            var accountJournal = await accountJournalObj.GetJournalByTypeAndCompany("payroll", slipRun.CompanyId);
            //if (accountJournal == null)
            //    accountJournal = await slipObj.InsertAccountJournalIfNotExists();

            //var acc334 = accountObj.SearchQuery(x => x.CompanyId == CompanyId && x.Code == "334").FirstOrDefault();
            //if (acc334 == null)
            //{
            //    var currentLiabilities = await irModelDataObj.GetRef<AccountAccountType>("account.data_account_type_current_liabilities");
            //    acc334 = new AccountAccount
            //    {
            //        Name = "Phải trả người lao động",
            //        Code = "334",
            //        InternalType = currentLiabilities.Type,
            //        UserTypeId = currentLiabilities.Id,
            //        CompanyId = CompanyId,
            //    };

            //    await accountObj.CreateAsync(acc334);
            //}

            var acc334 = await accountObj.GetAccount334CurrentCompany();

            //tạo 1 move cho toàn bộ bảng lương
            var move = new AccountMove
            {
                JournalId = accountJournal.Id,
                Journal = accountJournal,
                CompanyId = slipRun.CompanyId
            };

            // tạo moveline cho từng phiếu lương, 1 phiếu 2 line
            var lines = new List<AccountMoveLine>();
            var accountingDate = new DateTime(slipRun.Date.Value.Year, slipRun.Date.Value.Month, DateTime.DaysInMonth(slipRun.Date.Value.Year, slipRun.Date.Value.Month));
            foreach (var slip in slipRun.Slips)
            {
                var grossSalary = slip.TotalSalary.GetValueOrDefault();
                var taxSalary = slip.Tax.GetValueOrDefault();
                var bhxhSalary = slip.SocialInsurance.GetValueOrDefault();
                if (grossSalary == 0)
                    continue;

                var items = new List<AccountMoveLine>()
                {
                    new AccountMoveLine
                    {
                        Name =  "Lương tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                        Debit = 0,
                        Credit = grossSalary,
                        AccountId = acc334.Id,
                        Account = acc334,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                        Date = accountingDate,
                    },
                    new AccountMoveLine
                    {
                        Name = "Lương tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                        Debit = grossSalary,
                        Credit = 0,
                        AccountId = accountJournal.DefaultDebitAccount.Id,
                        Account = accountJournal.DefaultDebitAccount,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                        Date = accountingDate,
                    },
                };

                if (taxSalary > 0)
                {
                    //dùng tài khoản thuế thu nhập cá nhân
                    var acc3335 = await accountObj.GetAccount3335CurrentCompany();
                    items.Add(new AccountMoveLine
                    {
                        Name = "Thuế thu nhập cá nhân tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                        Debit = taxSalary,
                        Credit = 0,
                        AccountId = acc334.Id,
                        Account = acc334,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                        Date = accountingDate,
                    });

                    items.Add(new AccountMoveLine
                    {
                        Name = "Thuế thu nhập cá nhân tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                        Debit = 0,
                        Credit = taxSalary,
                        AccountId = acc3335.Id,
                        Account = acc3335,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                        Date = accountingDate,
                    });
                }

                if (bhxhSalary > 0)
                {
                    //dùng tài khoản thuế thu nhập cá nhân
                    var acc3383 = await accountObj.GetAccount3383CurrentCompany();
                    items.Add(new AccountMoveLine
                    {
                        Name = "Bảo hiểm xã hội tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                        Debit = bhxhSalary,
                        Credit = 0,
                        AccountId = acc334.Id,
                        Account = acc334,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                        Date = accountingDate,
                    });

                    items.Add(new AccountMoveLine
                    {
                        Name = "Bảo hiểm xã hội tháng " + slipRun.Date.Value.ToString("MM/yyyy"),
                        Debit = 0,
                        Credit = bhxhSalary,
                        AccountId = acc3383.Id,
                        Account = acc3383,
                        PartnerId = slip.Employee.PartnerId,
                        Move = move,
                        Date = accountingDate,
                    });
                }

                lines.AddRange(items);
            }
            move.Lines = lines;

            return move;
        }

        public async Task ActionDone(IEnumerable<Guid> ids)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id) && x.State == "confirm" && x.CompanyId == CompanyId).Include(x => x.Slips).ToListAsync();

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
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id) && x.CompanyId == CompanyId).Include("Slips.SalaryPayment")
                .Include(x => x.Move).ThenInclude(x => x.Lines).ToListAsync();

            var listMove = new List<AccountMove>();
            var listMoveLine = new List<AccountMoveLine>();
            foreach (var run in payslipruns)
            {
                if (run.Slips.Any(x => x.SalaryPayment != null)) throw new Exception("Đã có phiếu lương được chi lương nên không thể hủy!");
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
            var emps = await employeeObj.SearchQuery(x => x.Active == true && x.CompanyId == CompanyId).ToListAsync();
            var empIds = emps.Select(x => x.Id).ToList();
            var paysliprun = await SearchQuery(x => x.Id == id && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            var allChamcongs = await ccObj.SearchQuery(c => empIds.Any(x => x == c.EmployeeId)
            && c.Date.Value.Month == paysliprun.Date.Value.Month
            && c.Date.Value.Year == paysliprun.Date.Value.Year && c.CompanyId == CompanyId).ToListAsync();
            var firstDayOfMonth = new DateTime(paysliprun.Date.Value.Year, paysliprun.Date.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var commissions = await commissionObj.GetReport(new CommissionSettlementFilterReport() { CompanyId = paysliprun.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth });
            var Alladvances = await advanceObj.SearchQuery(c => empIds.Contains(c.EmployeeId.Value)
            && c.Type == "advance" && c.State == "done"
            && c.Date.Month == paysliprun.Date.Value.Month
            && c.Date.Year == paysliprun.Date.Value.Year && c.CompanyId == CompanyId).ToListAsync();

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
            var soCongNguyenNgay = chamCongs.Where(x => x.Type == "work").Sum(x => 1M);
            var soCongNuaNgay = chamCongs.Where(x => x.Type == "halfaday").Sum(x => 0.5M);
            var soCongNghi = chamCongs.Where(x => x.Type == "off").Sum(x => 1M);
            var soCongThucTe = soCongNguyenNgay + soCongNuaNgay;

            var emp = payslip.Employee;
            if (emp == null)
            {
                emp = await empObj.GetByIdAsync(payslip.EmployeeId);
            }
            var soCongChuanThang = DateTime.DaysInMonth(date.Value.Year, date.Value.Month) - emp.LeavePerMonth.GetValueOrDefault();

            if (chamCongs == null)
            {
                chamCongs = await ccObj.SearchQuery(c => c.EmployeeId == emp.Id
            && c.Date.Value.Month == date.Value.Month
            && c.Date.Value.Year == date.Value.Year && c.CompanyId == CompanyId).ToListAsync();
            }
            if (commission == null)
            {
                var firstDayOfMonth = new DateTime(date.Value.Year, date.Value.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var allCommission = await commissionObj.GetReport(new CommissionSettlementFilterReport() { CompanyId = payslip.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth, EmployeeId = emp.Id });
                commission = allCommission.FirstOrDefault(x => x.EmployeeId == emp.Id);
            }
            if (advances == null)
            {
                advances = await advanceObj.SearchQuery(c => c.EmployeeId == emp.Id
             && c.Type == "advance" && c.State == "done" && c.Date.Month == date.Value.Month
            && c.Date.Year == date.Value.Year && c.CompanyId == CompanyId).ToListAsync();
            }

            payslip.DaySalary = (emp.Wage.GetValueOrDefault() / soCongChuanThang);

            payslip.WorkedDay = Math.Min(soCongThucTe, soCongChuanThang);

            payslip.ActualLeavePerMonth = soCongNghi + soCongNuaNgay;
            payslip.LeavePerMonthUnpaid = soCongChuanThang - payslip.WorkedDay;

            payslip.OverTimeDay = Math.Max(soCongThucTe - soCongChuanThang, 0);
            payslip.TotalBasicSalary = Math.Round(payslip.WorkedDay.GetValueOrDefault() * payslip.DaySalary.GetValueOrDefault(), 0);

            payslip.OverTimeHour = chamCongs.Where(x => x.OverTime == true).Sum(x => x.OverTimeHour.GetValueOrDefault());
            payslip.OverTimeHourSalary = emp.RegularHour.GetValueOrDefault() == 0 ? 0 : Math.Round(((payslip.DaySalary.GetValueOrDefault() / (emp.RegularHour ?? 8)) * ((emp.OvertimeRate ?? 150) / 100) * payslip.OverTimeHour.GetValueOrDefault()), 0);

            payslip.OverTimeDaySalary = Math.Round((payslip.OverTimeDay.GetValueOrDefault() * payslip.DaySalary.GetValueOrDefault() * ((emp.RestDayRate ?? 100) / 100)), 0);
            payslip.Allowance = emp.Allowance.GetValueOrDefault();

            payslip.CommissionSalary = commission == null ? 0 : Math.Round(commission.Amount.GetValueOrDefault(), 0);

            payslip.TotalSalary = (payslip.TotalBasicSalary ?? 0) + (payslip.OverTimeHourSalary ?? 0) + (payslip.OverTimeDaySalary ?? 0) + (payslip.Allowance ?? 0)
               + payslip.OtherAllowance.GetValueOrDefault() + payslip.RewardSalary.GetValueOrDefault() + payslip.HolidayAllowance.GetValueOrDefault()
               + (payslip.CommissionSalary ?? 0) - (payslip.AmercementMoney ?? 0);
            payslip.AdvancePayment = advances.Sum(x => x.Amount);
            payslip.NetSalary = payslip.TotalSalary - (payslip.Tax ?? 0) - (payslip.SocialInsurance ?? 0);
        }

        public async Task ComputeSalaryByRunId(Guid id)
        {
            var paysliprun = await SearchQuery(x => x.Id == id && x.CompanyId == CompanyId).Include(x => x.Slips).ThenInclude(x => x.Employee).FirstOrDefaultAsync();

            //validate
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");


            if (!paysliprun.Slips.Any())
                return;

            // get all source
            var payslipObj = GetService<IHrPayslipService>();
            var employeeObj = GetService<IEmployeeService>();
            var ccObj = GetService<IChamCongService>();
            var commissionObj = GetService<ICommissionSettlementService>();
            var advanceObj = GetService<ISalaryPaymentService>();

            //kiểm tra có nhân viên mới hay không trước khi làm mới
            await CheckEmployeeCompany(paysliprun);

            var empIds = paysliprun.Slips.Select(x => x.EmployeeId);
            var allChamcongs = await ccObj.SearchQuery(c => empIds.Any(x => x == c.EmployeeId)
            && c.Date.Value.Month == paysliprun.Date.Value.Month
            && c.Date.Value.Year == paysliprun.Date.Value.Year && c.CompanyId == CompanyId).ToListAsync();
            var firstDayOfMonth = new DateTime(paysliprun.Date.Value.Year, paysliprun.Date.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var commissions = await commissionObj.GetReport(new CommissionSettlementFilterReport() { CompanyId = paysliprun.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth });
            var Alladvances = await advanceObj.SearchQuery(c => empIds.Contains(c.EmployeeId.Value)
            && c.Type == "advance" && c.State == "done"
            && c.Date.Month == paysliprun.Date.Value.Month
            && c.Date.Year == paysliprun.Date.Value.Year && c.CompanyId == CompanyId).ToListAsync();
            foreach (var item in paysliprun.Slips)
            {
                var chamCongs = allChamcongs.Where(x => x.EmployeeId == item.EmployeeId);
                var commission = commissions.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                if (commission == null) commission = new CommissionSettlementReportOutput();
                commission.Amount = item.CommissionSalary;
                var advance = Alladvances.Where(x => x.EmployeeId == item.EmployeeId);
                await ComputeSalary(item, chamCongs, commission, advance, paysliprun.Date);
            }


            await UpdateAsync(paysliprun);
        }

        public async Task<HrPayslipRunDisplay> CheckExist(DateTime date)
        {
            var run = await this.SearchQuery(x => x.Date.Value.Month == date.Month && x.Date.Value.Year == date.Year && x.CompanyId == CompanyId).FirstOrDefaultAsync();
            if (run == null) { return null; }
            return await this.GetHrPayslipRunForDisplay(run.Id);
        }

        public async Task CheckEmployeeCompany(HrPayslipRun paysliprun)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var employeeObj = GetService<IEmployeeService>();
            var empCompanys = await employeeObj.SearchQuery(x => x.Active == true && x.CompanyId == CompanyId).ToListAsync();
            var ccObj = GetService<IChamCongService>();
            var commissionObj = GetService<ICommissionSettlementService>();
            var advanceObj = GetService<ISalaryPaymentService>();
            var empHrPaySlipIds = paysliprun.Slips.Select(x => x.EmployeeId);
            var emps = empCompanys.Where(x => !empHrPaySlipIds.Contains(x.Id)).ToList();
            var empIds = emps.Select(x => x.Id).ToList();

            var allChamcongs = await ccObj.SearchQuery(c => empIds.Any(x => x == c.EmployeeId)
            && c.Date.Value.Month == paysliprun.Date.Value.Month
            && c.Date.Value.Year == paysliprun.Date.Value.Year && c.CompanyId == CompanyId).ToListAsync();
            var firstDayOfMonth = new DateTime(paysliprun.Date.Value.Year, paysliprun.Date.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var commissions = await commissionObj.GetReport(new CommissionSettlementFilterReport() { CompanyId = paysliprun.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth });
            var Alladvances = await advanceObj.SearchQuery(c => c.CompanyId == CompanyId && empIds.Contains(c.EmployeeId.Value) && c.Type == "advance" && c.State == "done" && c.Date.Month == paysliprun.Date.Value.Month && c.Date.Year == paysliprun.Date.Value.Year).ToListAsync();

            //tạo phiếu lương từ list emps
            var payslips = new List<HrPayslip>();
            if (emps.Any())
            {
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
            }

        }
    }

    public class PaySlipRunConfirmViewModel
    {
        public Guid? PayslipRunId { get; set; }

        public Guid? StructureId { get; set; }

        public IEnumerable<Guid> EmpIds { get; set; } = new List<Guid>();
    }
}
