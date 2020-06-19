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
        public async Task<IActionResult> Get([FromQuery]LaboOrderPaged val)
        {
            var result = await _laboOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboOrderService.GetLaboDisplay(id);
            return Ok(res);
        }

        [HttpPost]
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
        public async Task<IActionResult> Remove(Guid id)
        {
            await _laboOrderService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(LaboOrderDefaultGet val)
        {
            var res = await _laboOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
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
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _laboOrderService.GetPrint(id);
            res.OrderLines = res.OrderLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> QuickCreateOrderLine(LaboOrderLineQuickSave val)
        {

            return Ok();
        }

    }
}