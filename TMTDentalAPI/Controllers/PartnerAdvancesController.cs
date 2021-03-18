using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
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
    public class PartnerAdvancesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPartnerAdvanceService _partnerAdvanceService;
        private readonly IViewRenderService _view;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public PartnerAdvancesController(IMapper mapper, IPartnerAdvanceService partnerAdvanceService, IViewRenderService view, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _partnerAdvanceService = partnerAdvanceService;
            _view = view;
            _unitOfWork = unitOfWork;
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
            var res = await _partnerAdvanceService.GetPartnerAdvancePrint(id);

            var html = _view.Render("PartnerAdvance/Print", res);

            return Ok(new PrintData() { html = html });
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
