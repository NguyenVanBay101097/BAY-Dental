using System;
using System.Collections.Generic;
using System.IO;
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
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderPaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPaymentService _saleOrderPaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public SaleOrderPaymentsController(IMapper mapper, ISaleOrderPaymentService saleOrderPaymentService, IUnitOfWorkAsync unitOfWork, IViewRenderService viewRenderService,
            IPrintTemplateConfigService printTemplateConfigService,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService
            )
        {
            _mapper = mapper;
            _saleOrderPaymentService = saleOrderPaymentService;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaymentPaged val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistoryPartnerAdvance([FromQuery] HistoryPartnerAdvanceFilter val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultHistoryAdvanceAsync(val);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var saleOrderPayment = _saleOrderPaymentService.GetDisplay(id);

            _unitOfWork.Commit();

            return Ok(saleOrderPayment);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.SaleOrderPayment.Full")]
        public async Task<IActionResult> Create(SaleOrderPaymentSave val)
        {
            var saleOrderPayment = await _saleOrderPaymentService.CreateSaleOrderPayment(val);
            var basic = _mapper.Map<SaleOrderPaymentBasic>(saleOrderPayment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionPayment(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionPayment(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrderPayment.Full")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }



        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_account_payment" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_account_payment");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }
    }
}
