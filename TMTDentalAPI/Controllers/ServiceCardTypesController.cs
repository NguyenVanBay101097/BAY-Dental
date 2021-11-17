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
    public class ServiceCardTypesController : BaseApiController
    {
        private readonly IServiceCardTypeService _cardTypeService;
        private readonly IProductPricelistService _productPricelistService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ServiceCardTypesController(IServiceCardTypeService cardTypeService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IProductPricelistService productPricelistService)
        {
            _cardTypeService = cardTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _productPricelistService = productPricelistService;
        }

        [HttpGet]
        [CheckAccess(Actions = "ServiceCard.Type.Read")]
        public async Task<IActionResult> Get([FromQuery] ServiceCardTypePaged val)
        {
            var res = await _cardTypeService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "ServiceCard.Type.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).ThenInclude(x => x.Product.Categ).FirstOrDefaultAsync();
            if (type == null)
                return NotFound();

            return Ok(_mapper.Map<ServiceCardTypeDisplay>(type));
        }

        [HttpPost]
        [CheckAccess(Actions = "ServiceCard.Type.Create")]
        public async Task<IActionResult> Create(ServiceCardTypeSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var type = await _cardTypeService.CreateUI(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<ServiceCardTypeBasic>(type);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "ServiceCard.Type.Update")]
        public async Task<IActionResult> Update(Guid id, ServiceCardTypeSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _cardTypeService.UpdateUI(id, val);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "ServiceCard.Type.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id)
                .Include(x => x.ProductPricelist)
                .FirstOrDefaultAsync();

            if (type == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();

            if (type.ProductPricelist != null)
            {
                await _productPricelistService.DeleteAsync(type.ProductPricelist);
            }

            await _cardTypeService.DeleteAsync(type);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "ServiceCard.Type.Create")]
        public async Task<IActionResult> Create(CreateServiceCardTypeReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var cardType = new ServiceCardType
            {
                Name = val.Name,
                Period = val.Period,
                NbrPeriod = val.NbrPeriod,
                CompanyId = val.CompanyId,
            };

            var priceList = new ProductPricelist
            {
                Name = "Bảng giá " + cardType.Name,
                CompanyId = val.CompanyId,
            };

            //foreach (var item in val.ProductPricelistItems)
            //{
            //    priceList.Items.Add(new ProductPricelistItem
            //    {
            //        AppliedOn = "0_product_variant",
            //        ComputePrice = item.ComputePrice,
            //        PercentPrice = item.PercentPrice,
            //        FixedAmountPrice = item.FixedAmountPrice,
            //        ProductId = item.ProductId
            //    });
            //}

            await _productPricelistService.CreateAsync(priceList);

            cardType.ProductPricelistId = priceList.Id;
            await _cardTypeService.CreateAsync(cardType);
            _unitOfWork.Commit();
            var basic = _mapper.Map<ServiceCardTypeBasic>(cardType);
            return Ok(basic);
        }

        [HttpPut("{id}/[action]")]
        [CheckAccess(Actions = "ServiceCard.Type.Update")]
        public async Task<IActionResult> Update(Guid id, CreateServiceCardTypeReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var cardType = await _cardTypeService.SearchQuery(x => x.Id == id)
                .FirstOrDefaultAsync();
            _mapper.Map(val, cardType);
            //var priceList = cardType.ProductPricelist;
            //priceList.Name = "Bảng giá " + cardType.Name;

            //var itemsRemove = new List<ProductPricelistItem>();
            //foreach (var item in priceList.Items)
            //{
            //    if (!val.ProductPricelistItems.Any(x => x.Id == item.Id))
            //        itemsRemove.Add(item);
            //}

            //foreach (var item in itemsRemove)
            //    priceList.Items.Remove(item);

            //foreach (var item in val.ProductPricelistItems)
            //{
            //    if (!item.Id.HasValue || item.Id == Guid.Empty)
            //    {
            //        priceList.Items.Add(new ProductPricelistItem
            //        {
            //            AppliedOn = "0_product_variant",
            //            ComputePrice = item.ComputePrice,
            //            PercentPrice = item.PercentPrice,
            //            FixedAmountPrice = item.FixedAmountPrice,
            //            ProductId = item.ProductId
            //        });
            //    }
            //    else
            //    {
            //        var plItem = priceList.Items.Where(x => x.Id == item.Id).FirstOrDefault();
            //        plItem.ComputePrice = item.ComputePrice;
            //        plItem.PercentPrice = item.PercentPrice;
            //        plItem.FixedAmountPrice = item.FixedAmountPrice;
            //    }
            //}

            //await _productPricelistService.UpdateAsync(priceList);

            await _cardTypeService.UpdateAsync(cardType);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> AutoComplete(string search)
        {
            var res = await _cardTypeService.AutoCompleteSearch(search);
            return Ok(res);
        }

        [HttpPost("{id}/AddServices")]
        public async Task<IActionResult> AddProductPricelistItem(Guid id,[FromBody] List<Guid> productIds)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _cardTypeService.AddProductPricelistItem(id, productIds);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<IEnumerable<ProductPricelistItemDisplay>>(res));
        }

        [HttpPost("{id}/UpdateServices")]
        public async Task<IActionResult> UpdateProductPricelistItem(Guid id, [FromBody] List<ProductPricelistItemCreate> items)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardTypeService.UpdateProductPricelistItem(id, items);
            _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> ApplyServiceCategories(Guid id, [FromBody] List<ApplyServiceCategoryReq> vals)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardTypeService.ApplyServiceCategories(id, vals);
            _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> ApplyAllServices(Guid id, [FromBody] ApplyAllServiceReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardTypeService.ApplyAllServices(id, val);
            _unitOfWork.Commit();
            return Ok();
        }

    }
}