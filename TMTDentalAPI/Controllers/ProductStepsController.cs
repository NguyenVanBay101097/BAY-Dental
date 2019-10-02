using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStepsController : BaseApiController
    {
        private readonly IProductStepService _productStepService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductStepsController(IMapper mapper, IProductStepService productStepService, IProductService productService)
        {
            _productStepService = productStepService;
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet("{productId}")]
        public IActionResult GetByProductId(Guid productId)
        {
            var res = _productStepService.GetByProductId(productId);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductStepDisplay val)
        {
            if (val == null || !ModelState.IsValid)
            {
                return BadRequest();
            }
            var productStep = _mapper.Map<ProductStep>(val);

            await _productStepService.CreateAsync(productStep);

            return Ok(val);            
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductStepDisplay val,Guid id)
        {
            var pdStep = await _productStepService.GetByIdAsync(id);
            if (pdStep == null)
            {
                return NotFound();
            }

            pdStep = _mapper.Map(val, pdStep);
            await _productStepService.UpdateAsync(pdStep);


            return NoContent();
        }            
    }
}