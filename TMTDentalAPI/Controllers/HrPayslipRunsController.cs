using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrPayslipRunsController : BaseApiController
    {
        private readonly IHrPayslipRunService _payslipRunService;
        private readonly IMapper _mapper;
        private readonly IViewRenderService _view;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public HrPayslipRunsController(IHrPayslipRunService payslipRunService, IViewRenderService view, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _payslipRunService = payslipRunService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _view = view;
        }

        [HttpGet]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> Get([FromQuery] HrPayslipRunPaged val)
        {
            var result = await _payslipRunService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _payslipRunService.GetHrPayslipRunForDisplay(id);
            return Ok(res);
        }


        [HttpGet("HrPayslipRun/{id}/PrintAll")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Print")]
        public async Task<IActionResult> PrintAll(Guid id)
        {
            var res = await _payslipRunService.GetHrPayslipRunForDisplay(id);
            var html = _view.Render("SalaryEmployeePrint", res);
            return Ok(new printData() { html = html });
        }

        [HttpPost]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Create")]
        public async Task<IActionResult> Create(HrPayslipRunSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var payslipRun = await _payslipRunService.CreatePayslipRun(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<HrPayslipRunBasic>(payslipRun);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> Update(Guid id, HrPayslipRunSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.UpdatePayslipRun(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> ActionConfirm(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionConfirm(id);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> CreatePayslipByRunId(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.CreatePayslipByRunId(id);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]/{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> ComputeSalaryByRunId(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ComputeSalaryByRunId(id);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Update")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionDone(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Cancel")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionCancel(ids);

            _unitOfWork.Commit();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Salary.HrPayslipRun.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var paySlip = await _payslipRunService.SearchQuery(x => x.Id == id).Include(x => x.Slips).FirstOrDefaultAsync();
            if (paySlip == null)
                return BadRequest();

            if (paySlip.State != "draft")
                throw new Exception("Đợt lương ở trạng thái nháp mới có thể xóa");

            await _payslipRunService.DeleteAsync(paySlip);

            return NoContent();
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(HrPayslipRunDefaultGet val)
        {
            var date = DateTime.Now;
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var res = new HrPayslipRunSave();
            res.State = val.State;
            res.CompanyId = CompanyId;
            res.DateStart = startDate;
            res.DateEnd = endDate;
            res.Date = date;

            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CheckExist([FromQuery] DateTime? date)
        {
            if (date == null)
            {
                throw new Exception("phải chọn ngày để kiểm tra bảng lương");
            }
            var res = await _payslipRunService.CheckExist(date.Value);
            return Ok(res);
        }
    }
}