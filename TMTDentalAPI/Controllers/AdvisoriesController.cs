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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisoriesController : BaseApiController
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public AdvisoriesController(IAdvisoryService advisoryService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _advisoryService = advisoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AdvisoryPaged val)
        {
            var result = await _advisoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _advisoryService.GetAdvisoryDisplay(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdvisorySave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var advisory = await _advisoryService.CreateAdvisory(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<AdvisorySave>(advisory));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, AdvisorySave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _advisoryService.UpdateAdvisory(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _advisoryService.Unlink(id);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(AdvisoryDefaultGet val)
        {
            var res = await _advisoryService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetToothAdvise(AdvisoryToothAdvise val)
        {
            var res = await _advisoryService.GetToothAdvise(val);
            return Ok(res);
        }
    }
}
