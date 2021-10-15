using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/SaleOrders")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class SaleOrdersPublicController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderService _saleOrderService;

        public SaleOrdersPublicController(IMapper mapper, ISaleOrderService saleOrderService)
        {
            _mapper = mapper;
            _saleOrderService = saleOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] SaleOrderPublicFilter val)
        {
            var orders = await _saleOrderService.GetSaleOrdersByPartnerId(val.PartnerId);
            var res = _mapper.Map<IEnumerable<SaleOrderPublic>>(orders);
            return Ok(res);
        }
    }
}
