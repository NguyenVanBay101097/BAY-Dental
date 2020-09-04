using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            var res = await _mapper.ProjectTo<HrPayslipRunDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
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
            var paySlipRun = await SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (paySlipRun == null)
                throw new Exception("Đợt lương không tồn tại");

            paySlipRun = _mapper.Map(val, paySlipRun);

            await UpdateAsync(paySlipRun);
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
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id)).Include(x => x.Slips).ToListAsync();
            if (payslipruns == null)
                throw new Exception("Đợt lương không tồn tại");

            foreach (var run in payslipruns)
            {
                await payslipObj.ActionDone(run.Slips.Select(x => x.Id));
                run.State = "done";
            }

            await UpdateAsync(payslipruns);
        }

        public async Task ActionCancel(IEnumerable<Guid> ids)
        {
            var payslipObj = GetService<IHrPayslipService>();
            var payslipruns = await SearchQuery(x => ids.Contains(x.Id) && x.State == "done").Include(x => x.Slips).ToListAsync();
            if (payslipruns == null)
                throw new Exception("Đợt lương không tồn tại");

            foreach (var run in payslipruns)
            {
                await payslipObj.ActionCancel(run.Slips.Select(x => x.Id));
                //xóa các phiếu lương
                await payslipObj.DeleteAsync(run.Slips);

                run.State = "draft";
            }

            await UpdateAsync(payslipruns);
        }


    }

    public class PaySlipRunConfirmViewModel
    {
        public Guid? PayslipRunId { get; set; }

        public Guid? StructureId { get; set; }

        public IEnumerable<Guid> EmpIds { get; set; } = new List<Guid>();
    }
}
