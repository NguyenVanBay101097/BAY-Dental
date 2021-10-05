using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationsController : BaseApiController
    {
        private readonly IQuotationService _quotationService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;
        public QuotationsController(IViewRenderService viewRenderService, IUnitOfWorkAsync unitOfWork, IQuotationService quotationService
            , IPrintTemplateConfigService printTemplateConfigService,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService)
        {
            _quotationService = quotationService;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] QuotationPaged val)
        {
            var res = await _quotationService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _quotationService.GetByIdAsync(id);
            if (model == null || !ModelState.IsValid)
                return BadRequest();

            var res = await _quotationService.GetDisplay(id);
            return Ok(res);
        }

        [HttpGet("[action]/{partnerId}")]
        public async Task<IActionResult> GetDefault(Guid partnerId)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var res = await _quotationService.GetDefault(partnerId);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuotationSave val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationService.CreateAsync(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, QuotationSave val)
        {
            var model = await _quotationService.GetByIdAsync(id);
            if (model == null || !ModelState.IsValid)
                return NotFound();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.UpdateAsync(id, val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotionUsageCode(ApplyPromotionUsageCode val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationService.ApplyPromotionUsageCode(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyDiscountOnQuotation(ApplyDiscountViewModel val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.ApplyDiscountOnQuotation(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotion(ApplyPromotionRequest val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.ApplyPromotionOnQuotation(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> CreateSaleOrderByQuotation(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _quotationService.CreateSaleOrderByQuotation(id);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]     
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _quotationService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> Print(Guid id)
        {          
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_quotation" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_quotation");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }
    }
}
