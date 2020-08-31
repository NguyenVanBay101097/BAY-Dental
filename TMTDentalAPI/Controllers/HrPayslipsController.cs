using System;
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

            await _unitOfWork.BeginTransactionAsync();
           var res = await _payslipService.CreatePayslip(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<HrPayslipBasic>(res);
            return Ok(basic);
        }

     

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayslipSave val)
        {           

            await _unitOfWork.BeginTransactionAsync();    
            
            await _payslipService.UpdatePayslip(id,val);
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

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _payslipService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
