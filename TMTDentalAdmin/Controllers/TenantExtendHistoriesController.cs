using System;
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



        //[HttpPost]
        //public async Task<IActionResult> CreateAsync(TenantExtendHistorySave val)
        //{
        //    if (val == null || !ModelState.IsValid)
        //        return BadRequest();

        //   var res = await _tenantExtendHistoryService.CreateAsync(val);

        //    return Ok(_mapper.Map<TenantExtendHistoryDisplay>(res));
        //}

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
            var models = await _tenantExtendHistoryService.SearchQuery(x => x.TenantId == tenantId).OrderByDescending(x => x.StartDate).ToListAsync();
            var res = _mapper.Map<IEnumerable<TenantExtendHistoryDisplay>>(models);
            return Ok(res);
        }
    }
}
