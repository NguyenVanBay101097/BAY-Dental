using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardTypesController : BaseApiController
    {
        private readonly ICardTypeService _cardTypeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IProductPricelistService _productPricelistService;
        public CardTypesController(ICardTypeService cardTypeService,
            IProductPricelistService productPricelistService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _cardTypeService = cardTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _productPricelistService = productPricelistService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Card.Type.Read")]
        public async Task<IActionResult> Get([FromQuery] CardTypePaged val)
        {
            var res = await _cardTypeService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Card.Type.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.Pricelist).Include(x => x.Pricelist.Items).ThenInclude(x => x.Product.Categ).FirstOrDefaultAsync();
            if (type == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CardTypeDisplay>(type));
        }

        [HttpPost]
        [CheckAccess(Actions = "Card.Type.Create")]
        public async Task<IActionResult> Create(CardTypeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var entity = _mapper.Map<CardType>(val);
            //tạo pricelist
            var priceList = new ProductPricelist
            {
                Name = "Bảng giá " + entity.Name,
                CompanyId = CompanyId,
            };

            foreach (var item in val.ProductPricelistItems)
            {
                var prItem = _mapper.Map<ProductPricelistItem>(item);
                prItem.AppliedOn = "0_product_variant";
                priceList.Items.Add(prItem);
            }
            await _productPricelistService.CreateAsync(priceList);

            entity.PricelistId = priceList.Id;
            //tạo loại thẻ
            await _cardTypeService.CreateAsync(entity);
            _unitOfWork.Commit();

            var basic = _mapper.Map<CardTypeDisplay>(entity);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Card.Type.Update")]
        public async Task<IActionResult> Update(Guid id, CardTypeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var entity = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.Pricelist.Items).FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            entity = _mapper.Map(val, entity);
            var serviceItems = _mapper.Map<IEnumerable<ProductPricelistItem>>(val.ProductPricelistItems);
            //tạo pricelist
            var priceList = entity.Pricelist;
            if (priceList != null)
                priceList.Name = "Bảng giá " + entity.Name;

            var itemsRemove = new List<ProductPricelistItem>();
            foreach (var item in priceList.Items)
            {
                if (!val.ProductPricelistItems.Any(x => x.Id == item.Id))
                    itemsRemove.Add(item);
            }

            foreach (var item in itemsRemove)
                priceList.Items.Remove(item);

            foreach (var item in val.ProductPricelistItems)
            {
                if (!item.Id.HasValue || item.Id == Guid.Empty)
                {
                    var prItem = _mapper.Map<ProductPricelistItem>(item);
                    prItem.AppliedOn = "0_product_variant";
                    priceList.Items.Add(prItem);
                }
                else
                {
                    var plItem = priceList.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    _mapper.Map(item, plItem);
                }
            }

            await _productPricelistService.UpdateAsync(priceList);
            //update loại thẻ
            await _cardTypeService.UpdateAsync(entity);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Card.Type.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.Pricelist.Items).FirstOrDefaultAsync();
            if (type == null)
                return NotFound();
            if (type.Pricelist != null)
                await _productPricelistService.DeleteAsync(type.Pricelist);

            await _cardTypeService.DeleteAsync(type);

            return NoContent();
        }
    }
}