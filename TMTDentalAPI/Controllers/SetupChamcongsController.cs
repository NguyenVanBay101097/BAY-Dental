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
using SendGrid.Helpers.Mail;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupChamcongsController : BaseApiController
    {

        private readonly ISetupChamcongService _setupchamCongService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public SetupChamcongsController(ISetupChamcongService setupChamcongService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _setupchamCongService = setupChamcongService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rs = await _setupchamCongService.GetByCompanyId();
            return Ok(_mapper.Map<SetupChamcongDisplay>(rs));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SetupChamcongDisplay val)
        {
            var entity = _mapper.Map<SetupChamcong>(val);
            entity.CompanyId = await _setupchamCongService.GetCurrentCompanyId();
            await _setupchamCongService.CreateAsync(entity);
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SetupChamcongDisplay val)
        {
            var entity = await _setupchamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            entity = _mapper.Map(val, entity);
            entity.CompanyId = await _setupchamCongService.GetCurrentCompanyId();
            await _setupchamCongService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var entity = await _setupchamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            await _setupchamCongService.DeleteAsync(entity);
            return NoContent();
        }
    }
}
