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
    public class PartnerAdvancesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPartnerAdvanceService _partnerAdvanceService;
        private readonly IViewRenderService _view;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;
        public PartnerAdvancesController(IMapper mapper, IPartnerAdvanceService partnerAdvanceService, IViewRenderService view, IUnitOfWorkAsync unitOfWork,
             IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService
            , IPrintTemplateConfigService printTemplateConfigService)
        {
            _mapper = mapper;
            _partnerAdvanceService = partnerAdvanceService;
            _view = view;
            _unitOfWork = unitOfWork;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Read")]
        public async Task<IActionResult> Get([FromQuery] PartnerAdvancePaged val)
        {
            var result = await _partnerAdvanceService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _partnerAdvanceService.GetDisplayById(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Read")]
        public async Task<IActionResult> DefaultGet(PartnerAdvanceDefaultFilter val)
        {
            var res = await _partnerAdvanceService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Read")]
        public async Task<IActionResult> GetSummary(PartnerAdvanceSummaryFilter val)
        {
            var result = await _partnerAdvanceService.GetSummary(val);
            return Ok(result);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Create")]
        public async Task<IActionResult> Create(PartnerAdvanceSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var partnerAdvance = await _partnerAdvanceService.CreatePartnerAdvance(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<PartnerAdvanceBasic>(partnerAdvance);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Update")]
        public async Task<IActionResult> Update(Guid id, PartnerAdvanceSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _partnerAdvanceService.UpdatePartnerAdvance(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Update")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _partnerAdvanceService.ActionConfirm(ids);

            _unitOfWork.Commit();

            return NoContent();
        }



        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _partnerAdvanceService.GetByIdAsync(id);
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == (res.Type == "advance" ? "tmp_partner_advance" : "tmp_partner_refund") && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>(res.Type == "advance" ? "base.print_template_partner_advance" : "base.print_template_partner_refund");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.PartnerAdvance.Delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var partnerAdvance = await _partnerAdvanceService.GetByIdAsync(id);
            if (partnerAdvance.State != "draft")
                throw new Exception("Bạn không thể xóa phiếu khi đã xác nhận");

            if (partnerAdvance == null)
                return NotFound();

            await _partnerAdvanceService.DeleteAsync(partnerAdvance);
            return NoContent();
        }
    }
}
