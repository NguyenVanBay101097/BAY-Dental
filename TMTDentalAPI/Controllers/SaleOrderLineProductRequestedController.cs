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
    public class SaleOrderLineProductRequestedController : BaseApiController
    {
        private readonly ISaleOrderLineProductRequestedService _lineRequestedService;
        private readonly IMapper _mapper;
        public SaleOrderLineProductRequestedController(ISaleOrderLineProductRequestedService lineRequestedService,
            IMapper mapper)
        {
            _lineRequestedService = lineRequestedService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderLineProductRequestedPaged val)
        {
            var res = await _lineRequestedService.GetPagedResultAsync(val);
            return Ok(res);
        }

    }
}
