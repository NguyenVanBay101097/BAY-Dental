using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class SaleOrdersController : ControllerBase
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;

        public SaleOrdersController(ISaleOrderService saleOrderService, IMapper mapper)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int pageNumber = 1,
          int pageSize = 20,
          string orderBy = "name",
          string orderDirection = "asc",
          string filter = "")
        {
            var pageIndex = pageNumber - 1;

            var result = await _saleOrderService.GetPagedResultAsync(pageIndex, pageSize, orderBy, orderDirection, filter);

            var paged = new PagedResult<SaleOrderBasic>(result.TotalItems, pageNumber, pageSize)
            {
                //Có thể dùng thư viện automapper
                Items = _mapper.Map<IEnumerable<SaleOrderBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var saleOrder = await _saleOrderService.GetSaleOrderForDisplayAsync(id);
            if (saleOrder == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<SaleOrderDisplay>(saleOrder));
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleOrderDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var order = _mapper.Map<SaleOrder>(val);
            order.OrderLines = _mapper.Map<ICollection<SaleOrderLine>>(val.OrderLines);
            await _saleOrderService.CreateOrderAsync(order);

            val.Id = order.Id;
            return CreatedAtAction(nameof(Get), new { id = order.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaleOrderDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var order = await _saleOrderService.GetSaleOrderWithLines(id);
            if (order == null)
                return NotFound();

            order = _mapper.Map(val, order);

            SaveOrderLines(val, order);

            await _saleOrderService.UpdateOrderAsync(order);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var saleOrder = await _saleOrderService.GetSaleOrderByIdAsync(id);
            if (saleOrder == null)
                return NotFound();
            await _saleOrderService.UnlinkSaleOrderAsync(saleOrder);

            return NoContent();
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _saleOrderService.DefaultGet();
            return Ok(res);
        }

        [HttpPost("DefaultLineGet")]
        public async Task<IActionResult> DefaultLineGet(SaleOrderLineDefaultGet val)
        {
            var res = await _saleOrderService.DefaultLineGet(val);
            return Ok(res);
        }

        private void SaveOrderLines(SaleOrderDisplay val, SaleOrder order)
        {
            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    order.OrderLines.Add(_mapper.Map<SaleOrderLine>(line));
                }
                else
                {
                    _mapper.Map(line, order.OrderLines.SingleOrDefault(c => c.Id == line.Id));
                }
            }
        }
    }
}