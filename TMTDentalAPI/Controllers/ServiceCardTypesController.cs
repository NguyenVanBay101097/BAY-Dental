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
                Name = cardType.Name,
                CompanyId = val.CompanyId,
            };

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
                .Include(x => x.ProductPricelist)
                .FirstOrDefaultAsync();

            _mapper.Map(val, cardType);

            var priceList = cardType.ProductPricelist;
            priceList.Name = cardType.Name;
            await _productPricelistService.UpdateAsync(priceList);

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

        [HttpPost("AddServices")]
        public async Task<IActionResult> AddProductPricelistItem([FromBody] AddProductPricelistItem val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _cardTypeService.AddProductPricelistItem(val);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<ProductPricelistItemDisplay>(res));
        }

        [HttpPost("UpdateServices")]
        public async Task<IActionResult> UpdateProductPricelistItem([FromBody] UpdateProductPricelistItem val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardTypeService.UpdateProductPricelistItem(val);
            _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyServiceCategories([FromBody] ApplyServiceCategoryReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardTypeService.ApplyServiceCategories(val);
            _unitOfWork.Commit();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyAllServices([FromBody] ApplyAllServiceReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _cardTypeService.ApplyAllServices(val);
            _unitOfWork.Commit();
            return Ok();
        }

    }
}