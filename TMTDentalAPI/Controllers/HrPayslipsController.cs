﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
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
    public class HrPayslipsController : BaseApiController
    {
        private readonly IHrPayslipService _payslipService;
        private readonly IMapper _mapper;
        private readonly IHrPayslipLineService _hrPayslipLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayslipsController(IHrPayslipService payslipService, IHrPayslipLineService hrPayslipLineService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _hrPayslipLineService = hrPayslipLineService;
            _payslipService = payslipService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrPayslipPaged val)
        {
            var res = await _payslipService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _payslipService.GetHrPayslipDisplay(id);
            if (res == null)
                return NotFound();
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayslipSave val)
        {
            var payslip = _mapper.Map<HrPayslip>(val);
            SaveWorkedDayLines(val, payslip);

            await _unitOfWork.BeginTransactionAsync();
            await _payslipService.CreateAsync(payslip);
            _unitOfWork.Commit();

            var basic = _mapper.Map<HrPayslipBasic>(payslip);
            return Ok(basic);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayslipSave val)
        {
            var payslip = await _payslipService.SearchQuery(x => x.Id == id)
                .Include(x => x.WorkedDaysLines).FirstOrDefaultAsync();

            if (payslip == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            payslip = _mapper.Map(val, payslip);
            SaveWorkedDayLines(val, payslip);
            await _payslipService.UpdateAsync(payslip);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ComputeSheet(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _payslipService.ComputeSheet(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _payslipService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            await _payslipService.ActionDone(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeEmployee(HrPayslipOnChangeEmployee val)
        {
            var res = await _payslipService.OnChangeEmployee(val.EmployeeId, val.DateFrom, val.DateTo);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(HrPayslipDefaultGet val)
        {
            var res = _payslipService.DefaultGet(val);
            return Ok(res);
        }

        [HttpGet("{id}/WorkedDaysLines")]
        public async Task<IActionResult> GetWorkedDaysLines(Guid id)
        {
            var res = await _payslipService.GetWorkedDaysLines(id);
            return Ok(res);
        }

        [HttpGet("{id}/Lines")]
        public async Task<IActionResult> GetLines(Guid id)
        {
            var res = await _payslipService.GetLines(id);
            return Ok(res);
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> CancelCompute(Guid id)
        {
            var slip = await _payslipService.SearchQuery(x=>x.Id == id).Include(x=>x.Lines).FirstOrDefaultAsync();
            if (slip == null)
            {
                return NotFound();
            }
            slip.Lines.Clear();
            slip.State = "draft";
            slip.TotalAmount = 0;
            await _payslipService.UpdateAsync(slip);
            return NoContent();
        }
    }
}
