
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class PurchaseOrdersController : BaseApiController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork)
        {
            _purchaseOrderService = purchaseOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Purchase.Order.Read")]
        public async Task<IActionResult> Get([FromQuery]PurchaseOrderPaged val)
        {
            var result = await _purchaseOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Purchase.Order.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _purchaseOrderService.GetPurchaseDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Purchase.Order.Create")]
        public async Task<IActionResult> Create(PurchaseOrderDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            foreach (var item in val.OrderLines.ToList())
            {
                item.ProductUOM = null;
            }
            var labo = await _purchaseOrderService.CreateLabo(val);
            _unitOfWork.Commit();
            val.Id = labo.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Purchase.Order.Update")]
        public async Task<IActionResult> Update(Guid id, PurchaseOrderDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.UpdateLabo(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Purchase.Order.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _purchaseOrderService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(PurchaseOrderDefaultGet val)
        {
            var res = _purchaseOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Purchase.Order.Update")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.ButtonConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Purchase.Order.Cancel")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.ButtonCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Purchase.Order.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}