using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
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
    public class StockInventoriesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IStockInventoryService _stockInventoryService;
        private readonly IViewRenderService _view;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public StockInventoriesController(IMapper mapper, IStockInventoryService stockInventoryService, IViewRenderService view, IUnitOfWorkAsync unitOfWork,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService
            , IPrintTemplateConfigService printTemplateConfigService)
        {
            _mapper = mapper;
            _stockInventoryService = stockInventoryService;
            _view = view;
            _unitOfWork = unitOfWork;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
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

        [HttpGet("{id}/Print")]
        [CheckAccess(Actions = "Stock.Inventory.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_stock_inventory" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_stock_inventory");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
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
