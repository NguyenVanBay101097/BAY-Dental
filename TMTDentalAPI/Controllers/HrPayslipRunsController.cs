﻿using System;
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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrPayslipRunsController : BaseApiController
    {
        private readonly IHrPayslipRunService _payslipRunService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public HrPayslipRunsController(IHrPayslipRunService payslipRunService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _payslipRunService = payslipRunService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]HrPayslipRunPaged val)
        {
            var result = await _payslipRunService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _payslipRunService.GetHrPayslipRunForDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayslipRunSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var payslipRun = await _payslipRunService.CreatePayslipRun(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<HrPayslipRunBasic>(payslipRun);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayslipRunSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.UpdatePayslipRun(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionConfirm(PaySlipRunConfirmViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionConfirm(val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionDone(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _payslipRunService.ActionCancel(ids);

            _unitOfWork.Commit();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var paySlip = await _payslipRunService.SearchQuery(x => x.Id == id).Include(x=>x.Slips).FirstOrDefaultAsync();
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

            return Ok(res);
        }
    }
}