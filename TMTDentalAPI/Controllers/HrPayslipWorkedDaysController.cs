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
    public class HrPayslipWorkedDaysController : ControllerBase
    {
        private readonly IHrPayslipWorkedDayService _HrPayslipWorkedDayService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrPayslipWorkedDaysController(IHrPayslipWorkedDayService HrPayslipWorkedDayService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _HrPayslipWorkedDayService = HrPayslipWorkedDayService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] HrPayslipWorkedDayPaged val)
        {
            var res = await _HrPayslipWorkedDayService.GetPaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var HrPayslipWorkedDay = await _HrPayslipWorkedDayService.GetHrPayslipWorkedDayDisplay(id);
            if (HrPayslipWorkedDay == null)
                return NotFound();
            var res = _mapper.Map<HrPayslipWorkedDayDisplay>(HrPayslipWorkedDay);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrPayslipWorkedDaySave val)
        {
            var entitys = _mapper.Map<HrPayslipWorkedDays>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _HrPayslipWorkedDayService.CreateAsync(entitys);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<HrPayslipWorkedDayDisplay>(entitys));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrPayslipWorkedDaySave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var str = await _HrPayslipWorkedDayService.GetHrPayslipWorkedDayDisplay(id);
            if (str == null)
                return NotFound();

            str = _mapper.Map(val, str);

            await _HrPayslipWorkedDayService.UpdateAsync(str);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var HrPayslipWorkedDay = await _HrPayslipWorkedDayService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (HrPayslipWorkedDay == null)
            {
                return NotFound();
            }
            await _HrPayslipWorkedDayService.DeleteAsync(HrPayslipWorkedDay);
            return NoContent();
        }
    }
}
