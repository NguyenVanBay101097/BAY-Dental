using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Get(Guid? partnerId = null)
        {
            var query = _saleOrderService.SearchQuery(x => (!partnerId.HasValue || x.PartnerId == partnerId));
            var res = await _mapper.ProjectTo<SaleOrderPublic>(query).ToListAsync();
            return Ok(res);
        }
    }
}
