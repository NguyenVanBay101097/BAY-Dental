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
    public class HrPayslipLinesController : ControllerBase
    {
        private readonly IHrPayslipLineService _HrPayslipLineService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayslipLinesController(IHrPayslipLineService HrPayslipLineService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _HrPayslipLineService = HrPayslipLineService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrPayslipLinePaged val)
        {
            var res = await _HrPayslipLineService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrPayslipLine = await _HrPayslipLineService.GetHrPayslipLineDisplay(id);
            if (HrPayslipLine == null)
                return NotFound();
            var res = _mapper.Map<HrPayslipLineDisplay>(HrPayslipLine);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayslipLineSave val)
        {
            var entitys = _mapper.Map<HrPayslipLine>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _HrPayslipLineService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrPayslipLineDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayslipLineSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrPayslipLineService.GetHrPayslipLineDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);

            await _HrPayslipLineService.UpdateAsync(str);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var HrPayslipLine = await _HrPayslipLineService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (HrPayslipLine == null)
            {
                return NotFound();
            }
            await _HrPayslipLineService.DeleteAsync(HrPayslipLine);
            return NoContent();
        }
    }
}
