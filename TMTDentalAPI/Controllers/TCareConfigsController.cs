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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCareConfigsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ITCareConfigService _tcareConfigService;

        public TCareConfigsController(ITCareConfigService tcareConfigService, IMapper mapper)
        {
            _tcareConfigService = tcareConfigService;
            _mapper = mapper;
        }

        [HttpGet]
        [CheckAccess(Actions = "TCare.Config.Create")]
        public async Task<IActionResult> GetConfig()
        {
            var res = await _tcareConfigService.GetConfig();
            return Ok(res);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "TCare.Config.Create")]
        public async Task<IActionResult> Update(Guid id, TCareConfigSave config)
        {
            var entity = await _tcareConfigService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();

            entity = _mapper.Map(config, entity);
            CheckTime(entity);
            await _tcareConfigService.UpdateAsync(entity);
            return NoContent();
        }
        private void CheckTime(TCareConfig config)
        {
            var allowMinutes = new List<int>() { 15,30,45,60};
            if (!allowMinutes.Any(x => x == config.JobMessagingMinute) || !allowMinutes.Any(x => x == config.JobMessageMinute))
                throw new Exception("số phút chạy tự động không được cho phép!");
        }
    }
}
