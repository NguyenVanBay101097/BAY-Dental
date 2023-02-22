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
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockInventoryCriteriasController : BaseApiController
    {
        public readonly IUnitOfWorkAsync _unitOfWork;
        public readonly IMapper _mapper;
        public readonly IStockInventoryCriteriaService _SI_criteriaService;
        public StockInventoryCriteriasController(IMapper mapper, IUnitOfWorkAsync unitOfWork, IStockInventoryCriteriaService stockInventoryCriteriaService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _SI_criteriaService = stockInventoryCriteriaService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Stock.StockInventoryCriteria.Read")]
        public async Task<IActionResult> Get([FromQuery] StockInventoryCriteriaPaged val)
        {
            var result = await _SI_criteriaService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Stock.StockInventoryCriteria.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var employee = await _SI_criteriaService.GetByIdAsync(id);
            if (employee == null)
                return NotFound();
            return Ok(_mapper.Map<StockInventoryCriteriaDisplay>(employee));
        }

        [HttpPost]
        [CheckAccess(Actions = "Stock.StockInventoryCriteria.Create")]
        public async Task<IActionResult> Create(StockInventoryCriteriaSave val)
        {
            var entity = _mapper.Map<StockInventoryCriteria>(val);
            await _SI_criteriaService.CreateAsync(entity);
            return Ok(_mapper.Map<StockInventoryCriteriaDisplay>(entity));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Stock.StockInventoryCriteria.Update")]
        public async Task<IActionResult> Update(Guid id,StockInventoryCriteriaSave val)
        {
            var entity = await _SI_criteriaService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            _mapper.Map(val, entity);

            await _SI_criteriaService.UpdateAsync(entity);
            return Ok(_mapper.Map<StockInventoryCriteriaDisplay>(entity));
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Stock.StockInventoryCriteria.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _SI_criteriaService.GetByIdAsync(id);
            if (entity == null)
                return NotFound();
            await _SI_criteriaService.DeleteAsync(entity);

            return Ok();
        }
    }
}
