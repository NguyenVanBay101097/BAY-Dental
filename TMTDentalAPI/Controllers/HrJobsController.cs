using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrJobsController : BaseApiController
    {
        private readonly IHrJobService _hrJobService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrJobsController(IHrJobService hrJobService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _hrJobService = hrJobService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Hr.Job.Read")]
        public async Task<IActionResult> GetPaged([FromQuery] HrJobPaged val)
        {
            var res = await _hrJobService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Hr.Job.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _hrJobService.GetByIdAsync(id);

            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [HttpPost]
        [CheckAccess(Actions = "Hr.Job.Create")]
        public async Task<IActionResult> Create(HrJobSave val)
        {
            if (val == null|| !ModelState.IsValid)
                return BadRequest();

            var data = _mapper.Map<HrJob>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _hrJobService.CreateAsync(data);
            _unitOfWork.Commit();

            var basic = _mapper.Map<HrJobBasic>(data);

            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Hr.Job.Update")]
        public async Task<IActionResult> Update(Guid id, HrJobSave val)
        {
            var model = await _hrJobService.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            model = _mapper.Map(val, model);
            await _unitOfWork.BeginTransactionAsync();
            await _hrJobService.UpdateAsync(model);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Hr.Job.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var model = await _hrJobService.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            await _hrJobService.DeleteAsync(model);

            return NoContent();
        }
    }
}
