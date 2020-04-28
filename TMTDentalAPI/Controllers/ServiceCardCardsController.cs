using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ServiceCardCardsController : BaseApiController
    {
        private readonly IServiceCardCardService _cardCardService;
        private readonly ISaleOrderServiceCardCardRelService _orderCardRelService;
        private readonly IMapper _mapper;

        public ServiceCardCardsController(IServiceCardCardService cardCardService,
            ISaleOrderServiceCardCardRelService orderCardRelService,
            IMapper mapper)
        {
            _cardCardService = cardCardService;
            _orderCardRelService = orderCardRelService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ServiceCardCardPaged val)
        {
            var res = await _cardCardService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetHistories(Guid id)
        {
            var rels = await _orderCardRelService.SearchQuery(x => x.CardId == id, orderBy: s => s.OrderByDescending(m => m.DateCreated))
                .Include(x => x.Card)
                .Include(x => x.SaleOrder).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<SaleOrderServiceCardCardRelBasic>>(rels));
        }
    }
}