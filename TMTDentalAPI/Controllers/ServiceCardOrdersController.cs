using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCardOrdersController : BaseApiController
    {
        private readonly IServiceCardOrderService _cardOrderService;
        private readonly IMapper _mapper;

        public ServiceCardOrdersController(IServiceCardOrderService cardOrderService,
            IMapper mapper)
        {
            _cardOrderService = cardOrderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ServiceCardOrderPaged val)
        {
            var res = await _cardOrderService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCardOrderSave val)
        {
            var order = await _cardOrderService.CreateUI(val);

            var basic = _mapper.Map<ServiceCardOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ServiceCardOrderSave val)
        {
            await _cardOrderService.UpdateUI(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var order = await _cardOrderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            await _cardOrderService.DeleteAsync(order);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            return NoContent();
        }
    }
}