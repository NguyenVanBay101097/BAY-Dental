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
    public class CardCardsController : BaseApiController
    {
        private readonly ICardCardService _cardCardService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public CardCardsController(ICardCardService cardCardService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _cardCardService = cardCardService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CardCardPaged val)
        {
            var res = await _cardCardService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _cardCardService.SearchQuery(x => x.Id == id).Include(x => x.Type)
                .Include(x => x.Partner).FirstOrDefaultAsync();
            if (type == null)
                return NotFound();

            return Ok(_mapper.Map<CardCardDisplay>(type));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CardCardDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var type = _mapper.Map<CardCard>(val);
            await _cardCardService.CreateAsync(type);
            val.Id = type.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CardCardDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var type = await _cardCardService.GetByIdAsync(id);
            if (type == null)
                return NotFound();

            type = _mapper.Map(val, type);
            await _cardCardService.UpdateAsync(type);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _cardCardService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonConfirm(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonActive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonActive(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonCancel(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonReset(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonReset(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonLock(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonLock(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonUnlock(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _cardCardService.ButtonUnlock(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonUpgradeCard(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _cardCardService.ButtonUpgradeCard(ids);
            _unitOfWork.Commit();

            return NoContent();
        }
    }
}