using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCardTypesController : BaseApiController
    {
        private readonly IServiceCardTypeService _cardTypeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ServiceCardTypesController(IServiceCardTypeService cardTypeService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _cardTypeService = cardTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ServiceCardTypePaged val)
        {
            var res = await _cardTypeService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (type == null)
                return NotFound();

            return Ok(_mapper.Map<ServiceCardTypeDisplay>(type));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCardTypeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var type = await _cardTypeService.CreateUI(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<ServiceCardTypeBasic>(type);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ServiceCardTypeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _cardTypeService.UpdateUI(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _cardTypeService.GetByIdAsync(id);
            if (type == null)
                return NotFound();

            await _cardTypeService.DeleteAsync(type);

            return NoContent();
        }
    }
}