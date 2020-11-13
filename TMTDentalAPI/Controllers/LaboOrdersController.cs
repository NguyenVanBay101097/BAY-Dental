using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public LaboOrdersController(ILaboOrderService laboOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService)
        {
            _laboOrderService = laboOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.LaboOrder.Read")]
        public async Task<IActionResult> Get([FromQuery]LaboOrderPaged val)
        {
            var result = await _laboOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Basic.LaboOrder.Read")]
        public async Task<IActionResult> GetFromSaleOrder_OrderLine([FromQuery] LaboOrderPaged val)
        {
            var res = await _laboOrderService.GetFromSaleOrder_OrderLine(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.LaboOrder.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboOrderService.GetLaboDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.LaboOrder.Create")]
        public async Task<IActionResult> Create(LaboOrderDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var labo = await _laboOrderService.CreateLabo(val);
            _unitOfWork.Commit();
            val.Id = labo.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.LaboOrder.Update")]
        public async Task<IActionResult> Update(Guid id, LaboOrderDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
        
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.UpdateLabo(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.LaboOrder.Delete")]
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
        [CheckAccess(Actions = "Basic.LaboOrder.Update")]
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
        [CheckAccess(Actions = "Basic.LaboOrder.Cancel")]
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
        [CheckAccess(Actions = "Basic.LaboOrder.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _laboOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.LaboOrder.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _laboOrderService.GetPrint(id);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.LaboOrder.Statistics")] 
        public async Task<IActionResult> Statistics(LaboOrderStatisticsPaged val)
        {
            var result = await _laboOrderService.GetStatisticsPaged(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> LaboOrderReport(LaboOrderReportInput val)
        {
            var result = await _laboOrderService.GetLaboOrderReport(val);
            return Ok(result);
        }
    }
}