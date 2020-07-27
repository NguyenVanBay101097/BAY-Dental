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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PartnerTitlePaged val)
        {
            var result = await _partnerTitleService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _partnerTitleService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartnerTitleSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var result = _mapper.Map<PartnerTitle>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _partnerTitleService.CreateAsync(result);
            _unitOfWork.Commit();

            return CreatedAtAction(nameof(Get), new { id = result.Id }, val);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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
