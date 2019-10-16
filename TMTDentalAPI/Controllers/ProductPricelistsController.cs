using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductPricelistsController : BaseApiController
    {
        private readonly IProductPricelistService _pricelistService;
        private readonly IMapper _mapper;
        public ProductPricelistsController(IProductPricelistService pricelistService,
            IMapper mapper)
        {
            _pricelistService = pricelistService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ProductPricelistPaged val)
        {
            var result = await _pricelistService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductPricelistSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            var pricelist = _mapper.Map<ProductPricelist>(val);
            SaveItems(val, pricelist);
            await _pricelistService.CreateAsync(pricelist);

            var basic = _mapper.Map<ProductPricelistBasic>(pricelist);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductPricelistSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var pricelist = await _pricelistService.GetPriceListForUpdate(id);
            if (pricelist == null)
                return NotFound();

            pricelist = _mapper.Map(val, pricelist);
            SaveItems(val, pricelist);
            await _pricelistService.UpdateAsync(pricelist);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var pricelist = await _pricelistService.GetByIdAsync(id);
            if (pricelist == null)
                return NotFound();

            await _pricelistService.DeleteAsync(pricelist);
            return NoContent();
        }

        private void SaveItems(ProductPricelistSave val, ProductPricelist pricelist)
        {
            var existItems = pricelist.Items.ToList();
            var itemToRemoves = new List<ProductPricelistItem>();
            foreach (var existItem in existItems)
            {
                if (!val.Items.Any(x => x.Id == existItem.Id))
                    itemToRemoves.Add(existItem);
            }

            foreach (var item in itemToRemoves)
            {
                pricelist.Items.Remove(item);
            }

            int sequence = 0;
            foreach (var item in val.Items)
            {
                if (item.Id == Guid.Empty)
                {
                    var plItem = _mapper.Map<ProductPricelistItem>(item);
                    plItem.Sequence = sequence++;
                    pricelist.Items.Add(plItem);
                }
                else
                {
                    var plItem = pricelist.Items.SingleOrDefault(c => c.Id == item.Id);
                    _mapper.Map(item, plItem);
                    plItem.Sequence = sequence++;
                }
            }
        }
    }
}