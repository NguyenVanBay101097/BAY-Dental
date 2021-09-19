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
    public class AdvisoriesController : BaseApiController
    {
        private readonly IAdvisoryService _advisoryService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public AdvisoriesController(IAdvisoryService advisoryService, IViewRenderService viewRenderService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork, IPrintTemplateConfigService printTemplateConfigService,
             IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService)
        {
            _advisoryService = advisoryService;
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _unitOfWork = unitOfWork;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.Advisory.Read")]
        public async Task<IActionResult> Get([FromQuery] AdvisoryPaged val)
        {
            var result = await _advisoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.Advisory.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _advisoryService.GetAdvisoryDisplay(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAdvisoryLinesPaged([FromQuery] AdvisoryLinePaged val)
        {
            var res = await _advisoryService.GetAdvisoryLines(val);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.Advisory.Create")]
        public async Task<IActionResult> Create(AdvisorySave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var advisory = await _advisoryService.CreateAdvisory(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<AdvisoryBasic>(advisory));
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.Advisory.Update")]
        public async Task<IActionResult> Update(Guid id, AdvisorySave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _advisoryService.UpdateAdvisory(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.Advisory.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _advisoryService.RemoveAdvisory(id);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(AdvisoryDefaultGet val)
        {
            var res = await _advisoryService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetToothAdvise(AdvisoryToothAdvise val)
        {
            var res = await _advisoryService.GetToothAdvise(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPrint([FromQuery] IEnumerable<Guid> ids)
        {

            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_advisory" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_advisory");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, ids, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateSaleOrder(CreateFromAdvisoryInput val)
        {
            var res = await _advisoryService.CreateSaleOrder(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateQuotation(CreateFromAdvisoryInput val)
        {
            var res = await _advisoryService.CreateQuotation(val);
            return Ok(res);
        }
    }
}
