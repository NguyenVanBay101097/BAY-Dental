using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : BaseApiController
    {
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;

        public ProductCategoriesController(IProductCategoryService productCategoryService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService)
        {
            _productCategoryService = productCategoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductCategoryPaged val)
        {
            _modelAccessService.Check("ProductCategory", "Read");
            var res = await _productCategoryService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _modelAccessService.Check("ProductCategory", "Read");
            var category = await _productCategoryService.GetCategoryForDisplay(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductCategoryDisplay>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ProductCategory", "Create");
            var category = _mapper.Map<ProductCategory>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _productCategoryService.CreateAsync(category);
            _unitOfWork.Commit();

            val.Id = category.Id;
            val.CompleteName = category.CompleteName;
            return Ok(val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ProductCategoryDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("ProductCategory", "Update");
            var category = await _productCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _unitOfWork.BeginTransactionAsync();
            await _productCategoryService.Write(category);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            _modelAccessService.Check("ProductCategory", "Unlink");
            var category = await _productCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _productCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        public async Task<IActionResult> Autocomplete(ProductCategoryPaged val)
        {
            var res = await _productCategoryService.GetAutocompleteAsync(val);
            return Ok(res);
        }
    }
}