using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToaThuocLinesController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ToaThuocLinesController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangeProduct(ToaThuocLineOnChangeProductRequest val)
        {
            var res = new ToaThuocLineOnChangeProductResponse();
            if (val.ProductId.HasValue)
            {
                var product = await _productService.SearchQuery(x => x.Id == val.ProductId).Include(x => x.UOM).FirstOrDefaultAsync();
                res.UoM = _mapper.Map<UoMBasic>(product.UOM);
            }

            return Ok(res);
        }
    }
}
