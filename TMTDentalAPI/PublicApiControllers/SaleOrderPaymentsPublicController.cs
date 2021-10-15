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
    [Route("publicApi/SaleOrderPayments")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class SaleOrderPaymentsPublicController : ControllerBase
    {
        private readonly ISaleOrderPaymentService _orderPaymentService;
        private readonly IMapper _mapper;

        public SaleOrderPaymentsPublicController(ISaleOrderPaymentService orderPaymentService, IMapper mapper)
        {
            _orderPaymentService = orderPaymentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody] SaleOrderPaymentPublicFilter val)
        {
            var orderPayments = await _orderPaymentService.GetPaymentsByOrderId(val.SaleOrderId);
            var res = _mapper.Map<IEnumerable<SaleOrderPaymentPublic>>(orderPayments);
            return Ok(res);
        }
    }
}
