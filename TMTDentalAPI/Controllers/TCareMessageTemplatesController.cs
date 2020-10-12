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
    public class TCareMessageTemplatesController : BaseApiController
    {
        private readonly ITCareMessageTemplateService _TCareMessageTemplateService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public TCareMessageTemplatesController(ITCareMessageTemplateService TCareMessageTemplateService,
           IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _TCareMessageTemplateService = TCareMessageTemplateService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TCareMessageTemplatePaged val)
        {
            var mes = await _TCareMessageTemplateService.GetPaged(val);
            if (mes == null)
            {
                return NotFound();
            }
            return Ok(mes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var mes = await _TCareMessageTemplateService.GetDisplay(id);
            if (mes == null)
            {
                return NotFound();
            }
            return Ok(mes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TCareMessageTemplateSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var entity = _mapper.Map<TCareMessageTemplate>(val);
            var mes = await _TCareMessageTemplateService.CreateAsync(entity);
            var res = _mapper.Map<TCareMessageTemplateBasic>(mes);
            _unitOfWork.Commit();

            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, TCareMessageTemplateSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var entity = await _TCareMessageTemplateService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            entity = _mapper.Map(val, entity);
            await _TCareMessageTemplateService.UpdateAsync(entity);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var entity = await _TCareMessageTemplateService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            await _TCareMessageTemplateService.DeleteAsync(entity);

            return NoContent();
        }
    }
}
