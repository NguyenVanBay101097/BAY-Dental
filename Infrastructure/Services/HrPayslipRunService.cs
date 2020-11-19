﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
            var res = await _mapper.ProjectTo<HrPayslipRunDisplay>(SearchQuery(x => x.Id == id).Include(x=>x.Slips).ThenInclude(x=>x.Employee)).FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Đợt lương không tồn tại");        
            return res;
        }

        public async Task<HrPayslipRun> CreatePayslipRun(HrPayslipRunSave val)
        {
            var payslipRun = _mapper.Map<HrPayslipRun>(val);

            return await CreateAsync(payslipRun);
        }

        public async Task UpdatePayslipRun(Guid id, HrPayslipRunSave val)
        {
            var paySlipRun = await SearchQuery(x => x.Id == id).Include(x=>x.Slips).FirstOrDefaultAsync();
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
                        _mapper.Map(slip,sl);
                    }
                }
            }
        }

        public async Task ActionConfirm(PaySlipRunConfirmViewModel val)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var employeeObj = GetService<IEmployeeService>();
            var paysliprun = await SearchQuery(x => x.Id == val.PayslipRunId.Value).Include(x => x.Slips).FirstOrDefaultAsync();
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");

            var payslips = new List<HrPayslip>();
            foreach (var emp in val.EmpIds)
            {
                //lấy mặc định của 1 phiếu lương
                var payslip = new HrPayslip();
                payslip.CompanyId = paysliprun.CompanyId;
                payslip.DateFrom = paysliprun.DateStart;
                payslip.DateTo = paysliprun.DateEnd;
                payslip.EmployeeId = emp;
                payslip.PayslipRunId = paysliprun.Id;

                //sự kiện onchange employee trên payslip
                var changemp = await payslipObj.OnChangeEmployee(payslip.EmployeeId, payslip.DateFrom, payslip.DateTo);
                payslip.Name = changemp.Name;
                payslip.StructureTypeId = changemp.StructureTypeId;
                foreach (var item in changemp.WorkedDayLines)
                {
                    payslip.WorkedDaysLines.Add(new HrPayslipWorkedDays
                    {
                        Name = item.Name,
                        NumberOfDays = item.NumberOfDays,
                        NumberOfHours = item.NumberOfHours,
                        Amount = item.Amount,
                        WorkEntryTypeId = item.WorkEntryTypeId
                    });
                }

                //xử lý structure
                payslip.StructId = val.StructureId;
                if (!payslip.StructId.HasValue)
                {
                    if (!payslip.StructureTypeId.HasValue)
                    {
                        var employee = await employeeObj.GetByIdAsync(payslip.EmployeeId);
                        throw new Exception($"Nhân viên {employee.Name} chưa thiết lập loại mẫu lương");
                    }

                    var structureObj = GetService<IHrPayrollStructureService>();
                    var structure = await structureObj.SearchQuery(x => x.TypeId == payslip.StructureTypeId && x.RegularPay == true).FirstOrDefaultAsync();
                    if (structure == null)
                    {
                        var structureTypeObj = GetService<IHrPayrollStructureTypeService>();
                        var structureType = await structureTypeObj.GetByIdAsync(payslip.StructureTypeId);
                        throw new Exception($"Không tìm thấy mẫu lương thông dụng cho loại {structureType.Name}");
                    }
                      
                    payslip.StructId = structure.Id;
                }

                payslips.Add(payslip);
            }

            await payslipObj.CreateAsync(payslips);

            await payslipObj.ComputeSheet(payslips.Select(x => x.Id));

            paysliprun.State = "confirm";
            await UpdateAsync(paysliprun);
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
            var payslipObj = GetService<IHrPayslipService>();
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Slips).ToListAsync();

            foreach (var run in payslipruns)
            {
                await payslipObj.ActionCancel(run.Slips.Select(x => x.Id));
                await payslipObj.Unlink(run.Slips.Select(x => x.Id));
                run.State = "draft";
            }

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

        public async Task CreatePayslipByRunId(Guid id)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var employeeObj = GetService<IEmployeeService>();
            var ccObj = GetService<IChamCongService>();
            var commissionObj = GetService<ICommissionSettlementService>();

            //get all resource
            var emps = await employeeObj.SearchQuery(x=>x.Active == true).ToListAsync();
            var paysliprun = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            var allChamcongs = await ccObj.SearchQuery(c => emps.Select(x => x.Id).Any(x => x == c.EmployeeId)
            & c.Date.Value.Month == paysliprun.Date.Value.Month
            & c.Date.Value.Year == paysliprun.Date.Value.Year).ToListAsync();
            var firstDayOfMonth = new DateTime(paysliprun.Date.Value.Year, paysliprun.Date.Value.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var commissions = await commissionObj.GetReport(new CommissionSettlementReport() { CompanyId = paysliprun.CompanyId,DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth});

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
                var chamCongs = allChamcongs.Where(x=>x.EmployeeId == emp.Id);
                var commission = commissions.FirstOrDefault(x => x.EmployeeId == emp.Id);

                await ComputeSalary(payslip, chamCongs, commission, paysliprun.Date.Value);

                payslips.Add(payslip);
            }

            // lưu phiếu lương, bảng lương
            await payslipObj.CreateAsync(payslips);

            paysliprun.State = "confirm";
            await UpdateAsync(paysliprun);
        }

        public async Task ComputeSalary(HrPayslip payslip, IEnumerable<ChamCong> chamCongs = null, CommissionSettlementReportOutput commission = null, DateTime? date = null )
        {
            var empObj = GetService<IEmployeeService>();
            var ccObj = GetService<IChamCongService>();
            var commissionObj = GetService<ICommissionSettlementService>();
            date = date.HasValue ? date : DateTime.Now;

            var emp = payslip.Employee;
            if (emp == null)
            {
                emp = await empObj.GetByIdAsync(payslip.EmployeeId);
            }

            if (chamCongs == null)
            {
                chamCongs = await ccObj.SearchQuery(c => c.EmployeeId == emp.Id
            & c.Date.Value.Month == date.Value.Month
            & c.Date.Value.Year == date.Value.Year).ToListAsync();
            }
            if (commission == null)
            {
                var firstDayOfMonth = new DateTime(date.Value.Year, date.Value.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var allCommission = await commissionObj.GetReport(new CommissionSettlementReport() { CompanyId = payslip.CompanyId, DateFrom = firstDayOfMonth, DateTo = lastDayOfMonth, EmployeeId = emp.Id });
                commission = allCommission.FirstOrDefault(x=>x.EmployeeId == emp.Id);
            }

            payslip.DaySalary = Math.Round((emp.Wage.GetValueOrDefault() / (DateTime.DaysInMonth(date.Value.Year, date.Value.Month) - emp.LeavePerMonth.GetValueOrDefault())), 2);

            payslip.WorkedDay = chamCongs.Where(x => x.Type == "work").Count();

            var ngaythucnghi = emp.LeavePerMonth.GetValueOrDefault() - chamCongs.Where(x => x.Type == "off").Count() + chamCongs.Where(x => x.Type == "halfaday").Count() * 2;
            payslip.OverTimeDay = Math.Round(emp.LeavePerMonth.GetValueOrDefault() - ngaythucnghi, 2);
            payslip.OverTimeDay = payslip.OverTimeDay > 0 ? payslip.OverTimeDay : 0;
            payslip.TotalBasicSalary = Math.Round(((payslip.WorkedDay.GetValueOrDefault() - payslip.OverTimeDay.GetValueOrDefault()) * payslip.DaySalary.GetValueOrDefault()), 2);

            payslip.OverTimeHour = Math.Round(chamCongs.Where(x => x.OverTime == true).Sum(x => x.OverTimeHour.GetValueOrDefault()), 2);
            payslip.OverTimeHourSalary = emp.RegularHour.GetValueOrDefault() == 0 ? 0 : Math.Round(((payslip.DaySalary.GetValueOrDefault() / emp.RegularHour.GetValueOrDefault()) * (emp.OvertimeRate.GetValueOrDefault() / 100) * payslip.OverTimeHour.GetValueOrDefault()), 2);

            payslip.OverTimeDaySalary = Math.Round((payslip.OverTimeDay.GetValueOrDefault() * payslip.DaySalary.GetValueOrDefault() * (emp.RestDayRate.GetValueOrDefault() / 100)), 2);
            payslip.Allowance = emp.Allowance.GetValueOrDefault();

            payslip.TotalSalary = payslip.TotalBasicSalary + payslip.OverTimeHourSalary + payslip.OverTimeDaySalary + payslip.Allowance
                + payslip.OtherAllowance.GetValueOrDefault() + payslip.RewardSalary.GetValueOrDefault() + payslip.HolidayAllowance.GetValueOrDefault();

            payslip.CommissionSalary = commission == null ? 0 : Math.Round(commission.Amount.GetValueOrDefault(), 2);
            payslip.AmercementMoney = 0;
            payslip.NetSalary = payslip.TotalSalary + payslip.CommissionSalary - payslip.AmercementMoney.Value;

        }

        public async Task ComputeSalaryByRunId(Guid id)
        {
            var paysliprun = await SearchQuery(x => x.Id == id).Include(x=>x.Slips).ThenInclude(x=>x.Employee).FirstOrDefaultAsync();

            //validate
            if (paysliprun == null)
                throw new Exception("Đợt lương không tồn tại");
            foreach (var item in paysliprun.Slips)
            {
                await ComputeSalary(item,null,null, paysliprun.Date);
            }
            await UpdateAsync(paysliprun);
        }
    }

    public class PaySlipRunConfirmViewModel
    {
        public Guid? PayslipRunId { get; set; }

        public Guid? StructureId { get; set; }

        public IEnumerable<Guid> EmpIds { get; set; } = new List<Guid>();
    }
}
