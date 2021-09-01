using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaboWarrantiesController : BaseApiController
    {
        private readonly ILaboWarrantyService _laboWarrantyService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public LaboWarrantiesController(
            ILaboWarrantyService laboWarrantyService, 
            IMapper mapper,
            IUnitOfWorkAsync unitOfWork)
        {
            _laboWarrantyService = laboWarrantyService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] LaboWarrantyPaged val)
        {
            var result = await _laboWarrantyService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDefault(LaboWarrantyGetDefault val)
        {
            var res = await _laboWarrantyService.GetDefault(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboWarrantyService.GetLaboWarrantyDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LaboWarrantySave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var laboWarranty = await _laboWarrantyService.CreateLaboWarranty(val);
            _unitOfWork.Commit();
            return Ok(_mapper.Map<LaboWarrantyDisplay>(laboWarranty));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, LaboWarrantySave val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.UpdateLaboWarranty(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.ButtonConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.ButtonCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmSendWarranty(LaboWarrantyConfirm val)
        {
            if (val.Id == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.ConfirmSendWarranty(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CancelSendWarranty(Guid id)
        {
            if (id == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.CancelSendWarranty(id);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmReceiptInspection(LaboWarrantyConfirm val)
        {
            if (val.Id == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.ConfirmReceiptInspection(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CancelReceiptInspection(Guid id)
        {
            if (id == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.CancelReceiptInspection(id);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmAssemblyWarranty(LaboWarrantyConfirm val)
        {
            if (val.Id == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.ConfirmAssemblyWarranty(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CancelAssemblyWarranty(Guid id)
        {
            if (id == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.CancelAssemblyWarranty(id);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}
