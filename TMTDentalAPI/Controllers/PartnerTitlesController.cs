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
    public class PartnerTitlesController : BaseApiController
    {
        private readonly IPartnerTitleService _partnerTitleService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public PartnerTitlesController(IPartnerTitleService partnerTitleService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _partnerTitleService = partnerTitleService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet][CheckAccess(Actions = "Catalog.PartnerTitle.Read")]
        public async Task<IActionResult> Get([FromQuery] PartnerTitlePaged val)
        {
            var result = await _partnerTitleService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")][CheckAccess(Actions = "Catalog.PartnerTitle.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _partnerTitleService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("[action]")][CheckAccess(Actions = "Catalog.PartnerTitle.Read")]
        public async Task<IActionResult> Autocomplete(PartnerTitlePaged val)
        {
            var res = await _partnerTitleService.GetAutocompleteAsync(val);
            return Ok(res);
        }

        [HttpPost][CheckAccess(Actions = "Catalog.PartnerTitle.Create")]
        public async Task<IActionResult> Create(PartnerTitleSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var result = _mapper.Map<PartnerTitle>(val);
            await _unitOfWork.BeginTransactionAsync();
            var title = await _partnerTitleService.CreateAsync(result);
            _unitOfWork.Commit();

            var basic = _mapper.Map<PartnerTitleBasic>(title);
            return Ok(basic);
        }

        [HttpPut("{id}")][CheckAccess(Actions = "Catalog.PartnerTitle.Update")]
        public async Task<IActionResult> Update(Guid id, PartnerTitleSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();


            var result = await _partnerTitleService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            result = _mapper.Map(val, result);
            await _unitOfWork.BeginTransactionAsync();
            await _partnerTitleService.UpdateAsync(result);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")][CheckAccess(Actions = "Catalog.PartnerTitle.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _partnerTitleService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            await _partnerTitleService.DeleteAsync(result);

            return NoContent();
        }
    }
}
