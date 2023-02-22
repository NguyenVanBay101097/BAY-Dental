using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
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
    public class PartnerCategoriesController : BaseApiController
    {
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public PartnerCategoriesController(IPartnerCategoryService partnerCategoryService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _partnerCategoryService = partnerCategoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Read")]
        public async Task<IActionResult> Get([FromQuery] PartnerCategoryPaged val)
        {
            var result = await _partnerCategoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var category = await _partnerCategoryService.SearchQuery(x => x.Id == id).Include(x => x.Parent).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PartnerCategoryDisplay>(category));
        }

        [HttpPost]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Create")]
        public async Task<IActionResult> Create(PartnerCategoryDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<PartnerCategory>(val);
            await _unitOfWork.BeginTransactionAsync();
            category = await _partnerCategoryService.CreateAsync(category);
            _unitOfWork.Commit();

            return Ok(_mapper.Map<PartnerCategoryBasic>(category));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Update")]
        public async Task<IActionResult> Update(Guid id, PartnerCategoryDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var category = await _partnerCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category = _mapper.Map(val, category);
            await _unitOfWork.BeginTransactionAsync();
            await _partnerCategoryService.UpdateAsync(category);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var category = await _partnerCategoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            await _partnerCategoryService.DeleteAsync(category);

            return NoContent();
        }

        [HttpPost("Autocomplete")]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Read")]
        public async Task<IActionResult> Autocomplete(PartnerCategoryPaged val)
        {
            var result = await _partnerCategoryService.GetAutocompleteAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Catalog.PartnerCategory.Create")]
        public async Task<IActionResult> ActionImport(PartnerCategoryImportExcelViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var result = await _partnerCategoryService.Import(val);

            if (result.Success)
                _unitOfWork.Commit();

            return Ok(result);
        }
    }
}