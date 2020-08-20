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
        private readonly IHrPayslipService _HrPayslipService;
        private readonly IMapper _mapper;
        private readonly IHrPayslipLineService _hrPayslipLineService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayslipsController(IHrPayslipService HrPayslipService, IHrPayslipLineService hrPayslipLineService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _hrPayslipLineService = hrPayslipLineService;
            _HrPayslipService = HrPayslipService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrPayslipPaged val)
        {
            var res = await _HrPayslipService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrPayslip = await _HrPayslipService.GetHrPayslipDisplay(id);
            if (HrPayslip == null)
                return NotFound();
            var res = _mapper.Map<HrPayslipDisplay>(HrPayslip);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayslipSave val)
        {
            var entitys = _mapper.Map<HrPayslip>(val);
            entitys.CompanyId = CompanyId;
            await _unitOfWork.BeginTransactionAsync();
            await _HrPayslipService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrPayslipDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayslipSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrPayslipService.GetHrPayslipDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);
            str.CompanyId = CompanyId;

            await _HrPayslipService.UpdateAsync(str);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ComputePayslip(HrPayslipSave val)
        {
            var entity = _mapper.Map<HrPayslip>(val);
            var lines = (await _HrPayslipService.ComputePayslipLine(val.EmployeeId, val.DateFrom, val.DateTo)).ToList();
            foreach (var line in lines)
            {
                entity.Lines.Add(line);
            }
            entity.CompanyId = CompanyId;
            await _unitOfWork.BeginTransactionAsync();
            await _HrPayslipService.CreateAsync(entity);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrPayslipDisplay>(entity));
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> ComputePayslip(Guid id, HrPayslipSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrPayslipService.GetHrPayslipDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);
            if (str.Lines.Count() > 0)
              await _hrPayslipLineService.DeleteAsync(str.Lines);

            var lines = (await _HrPayslipService.ComputePayslipLine(val.EmployeeId, val.DateFrom, val.DateTo)).ToList();
            foreach (var line in lines)
            {
                str.Lines.Add(line);
            }
            str.CompanyId = CompanyId;

            await _HrPayslipService.UpdateAsync(str);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var HrPayslip = await _HrPayslipService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (HrPayslip == null)
            {
                return NotFound();
            }
            await _HrPayslipService.DeleteAsync(HrPayslip);
            return NoContent();
        }
    }
}
