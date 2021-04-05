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

        public AdvisoriesController(IAdvisoryService advisoryService, IViewRenderService viewRenderService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _advisoryService = advisoryService;
            _mapper = mapper;
            _viewRenderService = viewRenderService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AdvisoryPaged val)
        {
            var result = await _advisoryService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _advisoryService.GetAdvisoryDisplay(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAdvisoryLines([FromQuery] AdvisoryLinePaged val)
        {
            var res = await _advisoryService.GetAdvisoryLines(val);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost]
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
        public async Task<IActionResult> GetPrint([FromQuery] Guid customerId, [FromQuery] IEnumerable<Guid> ids)
        {

            var res = await _advisoryService.Print(customerId, ids);

            if (res == null)
                return NotFound();
            var html = _viewRenderService.Render("Advisory/Print", res);

            return Ok(new PrintData() { html = html });
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
