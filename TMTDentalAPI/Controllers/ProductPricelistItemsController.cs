using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace TMTDentalAPI.Controllers
{
    public class ProductPricelistItemsController : BaseApiController
    {
        private readonly IProductPricelistItemService _productPriceListItemService;
        private readonly IMapper _mapper;
        public ProductPricelistItemsController(IProductPricelistItemService productPricelistItemService, IMapper mapper)
        {
            _productPriceListItemService = productPricelistItemService;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var pricelist = await _productPriceListItemService.GetByIdAsync(id);
            if (pricelist == null)
                return NotFound();

            await _productPriceListItemService.DeleteAsync(pricelist);
            return NoContent();
        }
    }
}
