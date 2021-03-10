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
    public class StockInventoriesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IStockInventoryService _stockInventoryService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public StockInventoriesController(IMapper mapper, IStockInventoryService stockInventoryService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _stockInventoryService = stockInventoryService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Stock.Inventory.Read")]
        public async Task<IActionResult> Get([FromQuery] StockInventoryPaged val)
        {
            var result = await _stockInventoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Stock.Inventory.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _stockInventoryService.GetDisplay(id);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Stock.Inventory.Read")]
        public async Task<IActionResult> DefaultGet()
        {
            var res = await _stockInventoryService.DefaultGet();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Stock.Inventory.Create")]
        public async Task<IActionResult> Create(StockInventorySave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var inventory = await _stockInventoryService.CreateStockInventory(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<StockInventoryBasic>(inventory);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Stock.Inventory.Update")]
        public async Task<IActionResult> Update(Guid id, StockInventorySave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.UpdateStockInventory(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Stock.Inventory.Update")]
        public async Task<IActionResult> PrepareInventory(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.PrepareInventory(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Stock.Inventory.Read")]
        public async Task<IActionResult> GetInventoryLineByProductId(StockInventoryLineByProductId val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var res = await _stockInventoryService.InventoryLineByProductId(val);

            _unitOfWork.Commit();

            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Stock.Inventory.Update")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.ActionDone(ids);

            _unitOfWork.Commit();

            return NoContent();
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Stock.Inventory.Update")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.ActionCancel(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Stock.Inventory.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var inventory = await _stockInventoryService.GetByIdAsync(id);
            if (inventory.State != "draft")
                throw new Exception("Không thể xóa phiếu kiểm kho đang xử lý hoặc hoàn thành");

            if (inventory == null)
                return NotFound();

            await _stockInventoryService.DeleteAsync(inventory);
            return NoContent();
        }


    }
}
