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
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).FirstOrDefaultAsync();
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
        public async Task<IActionResult> Remove(Guid id)
        {
            var type = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).FirstOrDefaultAsync();
            if (type == null)
                return NotFound();
            await _productPricelistService.DeleteAsync(type.ProductPricelist);
            await _cardTypeService.DeleteAsync(type);

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create(CreateServiceCardTypeReq val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var entity = _mapper.Map<ServiceCardType>(val);
            var serviceItems = _mapper.Map<IEnumerable<ProductPricelistItem>>(val.ProductPricelistItems);
            //tạo pricelist
            _cardTypeService.SaveProductPricelistItem(entity, serviceItems);
            //tạo loại thẻ
            await _cardTypeService.CreateAsync(entity);
            _unitOfWork.Commit();

            var basic = _mapper.Map<ServiceCardTypeDisplay>(entity);
            return Ok(basic);
        }

        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> Update(Guid id, CreateServiceCardTypeReq val)
        {
            var entity = await _cardTypeService.SearchQuery(x => x.Id == id).Include(x => x.ProductPricelist.Items).FirstOrDefaultAsync();
            if (entity == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            entity = _mapper.Map(val, entity);
            var serviceItems = _mapper.Map<IEnumerable<ProductPricelistItem>>(val.ProductPricelistItems);
            //tạo pricelist
            _cardTypeService.SaveProductPricelistItem(entity, serviceItems);
            //update loại thẻ
            await _cardTypeService.UpdateAsync(entity);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> AutoComplete(string search)
        {
            var res = await _cardTypeService.AutoCompleteSearch(search);
            return Ok(res);
        }

    }
}