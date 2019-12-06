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
    public class SaleCouponsController : BaseApiController
    {
        private readonly ISaleCouponService _couponService;
        private readonly IMapper _mapper;

        public SaleCouponsController(ISaleCouponService couponService,
            IMapper mapper)
        {
            _couponService = couponService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SaleCouponPaged val)
        {
            var result = await _couponService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var display = await _couponService.GetDisplay(id);
            if (display == null)
                return NotFound();

            return Ok(display);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var program = await _couponService.GetByIdAsync(id);
            if (program == null)
                return NotFound();
            await _couponService.DeleteAsync(program);

            return NoContent();
        }
    }
}