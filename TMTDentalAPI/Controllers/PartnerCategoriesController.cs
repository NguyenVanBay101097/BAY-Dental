﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerCategoriesController : BaseApiController
    {
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IMapper _mapper;

        public PartnerCategoriesController(IPartnerCategoryService partnerCategoryService,
            IMapper mapper)
        {
            _partnerCategoryService = partnerCategoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]PartnerCategoryPaged val)
        {
            var result = await _partnerCategoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _partnerCategoryService.SearchQuery(x => x.Id == id).Include(x => x.Parent).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PartnerCategoryDisplay>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartnerCategoryDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<PartnerCategory>(val);
            await _partnerCategoryService.CreateAsync(category);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PartnerCategoryDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _partnerCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _partnerCategoryService.UpdateAsync2(category);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _partnerCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _partnerCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(PartnerCategoryPaged val)
        {
            var result = await _partnerCategoryService.GetAutocompleteAsync(val);
            return Ok(result);
        }
    }
}