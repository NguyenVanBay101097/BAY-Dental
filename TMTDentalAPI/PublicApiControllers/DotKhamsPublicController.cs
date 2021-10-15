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
    [Route("publicApi/Examinations")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class DotKhamsPublicController : ControllerBase
    {
        private readonly IDotKhamService _dotKhamService;
        private readonly IMapper _mapper;

        public DotKhamsPublicController(IDotKhamService dotKhamService, IMapper mapper)
        {
            _dotKhamService = dotKhamService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] DotKhamPublicFilter val)
        {
            var dotkhams = await _dotKhamService.GetDotKhamsBySaleOrderId(val.SaleOrderId);
            var res = _mapper.Map<IEnumerable<DotKhamPublic>>(dotkhams);
            return Ok(res);
        }
    }
}
