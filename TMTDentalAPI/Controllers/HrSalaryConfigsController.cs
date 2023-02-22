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
    public class HrSalaryConfigsController : BaseApiController
    {

        private readonly IHrSalaryConfigService _hrSalaryConfigService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public HrSalaryConfigsController(IHrSalaryConfigService hrSalaryConfigService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _hrSalaryConfigService = hrSalaryConfigService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var rs = await _hrSalaryConfigService.GetByCompanyId(CompanyId);
            return Ok(rs);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HrSalaryConfigSave val)
        {
            var entity = _mapper.Map<HrSalaryConfig>(val);
            await _hrSalaryConfigService.CreateAsync(entity);
            return Ok(_mapper.Map<HrSalaryConfigDisplay>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, HrSalaryConfigSave val)
        {
            var entity = await _hrSalaryConfigService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            entity = _mapper.Map(val, entity);
            await _hrSalaryConfigService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var entity = await _hrSalaryConfigService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null) return NotFound();
            await _hrSalaryConfigService.DeleteAsync(entity);
            return NoContent();
        }

    }
}
