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
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardTypesController : BaseApiController
    {
        private readonly ICardTypeService _cardTypeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public CardTypesController(ICardTypeService cardTypeService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _cardTypeService = cardTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CardTypePaged val)
        {
            var res = await _cardTypeService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.Pricelist).FirstOrDefaultAsync();
            if (type == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CardTypeDisplay>(type));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CardTypeDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var type = await _cardTypeService.CreateCardType(val);
            _unitOfWork.Commit();

            val.Id = type.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CardTypeDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _cardTypeService.UpdateCardType(id, val);
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