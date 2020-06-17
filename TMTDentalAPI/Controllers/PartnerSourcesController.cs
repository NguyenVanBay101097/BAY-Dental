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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerSourcesController : BaseApiController
    {
        private readonly IPartnerSourceService _partnerSourceService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public PartnerSourcesController(IPartnerSourceService partnerSourceService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _partnerSourceService = partnerSourceService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PartnerSourcePaged val)
        {
            var result = await _partnerSourceService.GetPagedResultAsync(val);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _partnerSourceService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartnerSourceSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var result = _mapper.Map<PartnerSource>(val);

            await _partnerSourceService.CreateAsync(result);

            var basic = _mapper.Map<PartnerSourceBasic>(result);

            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PartnerSourceSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            var source = await _partnerSourceService.GetByIdAsync(id);
            if (source == null)
                return NotFound();

            source = _mapper.Map(val, source);

            await _unitOfWork.BeginTransactionAsync();

            await _partnerSourceService.UpdateAsync(source);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var source = await _partnerSourceService.GetByIdAsync(id);
            if (source == null)
                return NotFound();
            await _partnerSourceService.DeleteAsync(source);

            return NoContent();
        }


    }
}