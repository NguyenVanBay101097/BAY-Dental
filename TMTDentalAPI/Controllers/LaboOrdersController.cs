using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboOrdersController : BaseApiController
    {
        private readonly ILaboOrderService _laboOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDotKhamService _dotKhamService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public LaboOrdersController(ILaboOrderService laboOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService, IViewRenderService viewRenderService,
            IPrintTemplateService printTemplateService,
              IIRModelDataService modelDataService
            , IPrintTemplateConfigService printTemplateConfigService)
        {
            _laboOrderService = laboOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> Get([FromQuery] LaboOrderPaged val)
        {
            var result = await _laboOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetFromSaleOrder_OrderLine([FromQuery] LaboOrderPaged val)
        {
            var res = await _laboOrderService.GetFromSaleOrder_OrderLine(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboOrderService.GetLaboDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Labo.LaboOrder.Create")]
        public async Task<IActionResult> Create(LaboOrderSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var labo = await _laboOrderService.CreateLabo(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<LaboOrderDisplay>(labo));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Labo.LaboOrder.Update")]
        public async Task<IActionResult> Update(Guid id, LaboOrderSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.UpdateLabo(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Labo.LaboOrder.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(LaboOrderDefaultGet val)
        {
            var res = await _laboOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Update")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.ButtonConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Cancel")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.ButtonCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Cancel")]
        public async Task<IActionResult> ActionCancelReceipt(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.ActionCancelReceipt(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/Print")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_labo_order" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_labo_orde");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> Statistics(LaboOrderStatisticsPaged val)
        {
            var result = await _laboOrderService.GetStatisticsPaged(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.OrderLabo.Read")]
        public async Task<IActionResult> LaboOrderGetCount(LaboOrderGetCount val)
        {
            var result = await _laboOrderService.GetCountLaboOrder(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetLaboForSaleOrderLine([FromQuery] LaboOrderPaged val)
        {
            var res = await _laboOrderService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.OrderLabo.Read")]
        public async Task<IActionResult> GetOrderLabo([FromQuery] OrderLaboPaged val)
        {

            var res = await _laboOrderService.GetPagedOrderLaboAsync(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.ExportLabo.Read")]
        public async Task<IActionResult> GetExportLabo([FromQuery] ExportLaboPaged val)
        {
            var res = await _laboOrderService.GetPagedExportLaboAsync(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.OrderLabo.Update")]
        public async Task<IActionResult> UpdateOrderLabo(LaboOrderReceiptSave val)
        {
            var labo = await _laboOrderService.GetByIdAsync(val.Id);
            labo.DateReceipt = val.DateReceipt;
            labo.WarrantyCode = val.WarrantyCode;
            labo.WarrantyPeriod = val.WarrantyPeriod;
            await _laboOrderService.UpdateAsync(labo);
            return NoContent();

        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.ExportLabo.Update")]
        public async Task<IActionResult> UpdateExportLabo(ExportLaboOrderSave val)
        {
            var labo = await _laboOrderService.GetByIdAsync(val.Id);
            labo.DateExport = val.DateExport.HasValue ? val.DateExport : null;
            await _laboOrderService.UpdateAsync(labo);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CheckExistWarrantyCode(LaboOrderCheck val)
        {
            var res = await _laboOrderService.CheckExistWarrantyCode(val);
            return Ok(res);
        }
    }
}