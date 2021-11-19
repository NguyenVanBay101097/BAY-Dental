using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ProductRequestsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IProductRequestService _productRequestService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly ISaleOrderLineService _saleLineService;

        public ProductRequestsController(IMapper mapper, IProductRequestService productRequestService, IUnitOfWorkAsync unitOfWork,
            ISaleOrderLineService saleLineService)
        {
            _mapper = mapper;
            _productRequestService = productRequestService;
            _unitOfWork = unitOfWork;
            _saleLineService = saleLineService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.ProductRequest.Read")]
        public async Task<IActionResult> Get([FromQuery] ProductRequestPaged val)
        {
            var result = await _productRequestService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.ProductRequest.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _productRequestService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.ProductRequest.Read")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _productRequestService.DefaultGet();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.ProductRequest.Create")]
        public async Task<IActionResult> Create(ProductRequestSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var request = await _productRequestService.CreateRequest(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<ProductRequestBasic>(request);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.ProductRequest.Update")]
        public async Task<IActionResult> Update(Guid id, ProductRequestSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _productRequestService.UpdateRequest(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.ProductRequest.Update")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _productRequestService.ActionConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.ProductRequest.Update")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _productRequestService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.ProductRequest.Done")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _productRequestService.ActionDone(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.ProductRequest.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = await _productRequestService.SearchQuery(x => x.Id == id)
                .Include(x => x.Lines)
                .FirstOrDefaultAsync();
            if (request == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            var saleLineIds = request.Lines.Where(x => x.SaleOrderLineId.HasValue).Select(x => x.SaleOrderLineId.Value).Distinct().ToList();
            await _productRequestService.DeleteAsync(request);
            if (saleLineIds.Any())
                await _saleLineService.ComputeProductRequestedQuantity(saleLineIds);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
