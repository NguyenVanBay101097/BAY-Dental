﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantExtendHistoriesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ITenantExtendHistoryService _tenantExtendHistoryService;
        public TenantExtendHistoriesController(IMapper mapper, ITenantExtendHistoryService tenantExtendHistoryService)
        {
            _mapper = mapper;
            _tenantExtendHistoryService = tenantExtendHistoryService;
        }



        [HttpPost]
        public async Task<IActionResult> CreateAsync(TenantExtendHistorySave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            if (val.StartDate > val.ExpirationDate)
                throw new Exception("Ngày bắt đầu không thể lớn hơn ngày hết hạn!");

            var res = _mapper.Map<TenantExtendHistory>(val);
            res = await _tenantExtendHistoryService.CreateAsync(res);

            return Ok(_mapper.Map<TenantExtendHistoryDisplay>(res));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var model = await _tenantExtendHistoryService.GetByIdAsync(id);
            if (model == null)
                return NotFound();

            await _tenantExtendHistoryService.DeleteAsync(model);
            return NoContent();
        }

        [HttpGet("{tenantId}/[action]")]
        public async Task<IActionResult> GetAllByTenantId(Guid tenantId)
        {
            var models = await _tenantExtendHistoryService.SearchQuery(x => x.TenantId == tenantId).OrderBy(x => x.DateCreated).ToListAsync();
            var res = _mapper.Map<IEnumerable<TenantExtendHistoryDisplay>>(models);
            return Ok(res);
        }

    }
}
