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
    [Route("publicApi/SaleOrderLines")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class SaleOrderLinesPublicController : ControllerBase
    {
        private readonly ISaleOrderLineService _orderLineService;
        private readonly IMapper _mapper;

        public SaleOrderLinesPublicController(ISaleOrderLineService orderLineService, IMapper mapper)
        {
            _orderLineService = orderLineService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] SaleOrderLinePublicFilter val)
        {
            var lines = await _orderLineService.GetOrderLinesBySaleOrderId(val.SaleOrderId);
            var res = _mapper.Map<IEnumerable<SaleOrderLinePublic>>(lines);
            return Ok(res);
        }
    }
}
