using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCardCardsController : BaseApiController
    {
        private readonly IServiceCardCardService _cardCardService;

        public ServiceCardCardsController(IServiceCardCardService cardCardService)
        {
            _cardCardService = cardCardService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ServiceCardCardPaged val)
        {
            var res = await _cardCardService.GetPagedResultAsync(val);
            return Ok(res);
        }
    }
}