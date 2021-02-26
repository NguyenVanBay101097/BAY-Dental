using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get([FromQuery] StockInventoryPaged val)
        {
            var result = await _stockInventoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _stockInventoryService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(StockInventoryDefaultGet val)
        {
            var res = await _stockInventoryService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StockInventorySave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var inventory = await _stockInventoryService.CreateStockInventory(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<StockInventoryBasic>(inventory);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, StockInventorySave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.UpdateStockInventory(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PrepareInventory(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.PrepareInventory(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetlistProductInventory(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var res = await _stockInventoryService.GetListProductInventory(id);

            _unitOfWork.Commit();

            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _stockInventoryService.ActionDone(ids);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
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
