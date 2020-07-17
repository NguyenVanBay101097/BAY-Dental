using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuThuChisController : BaseApiController
    {
        private readonly IPhieuThuChiService _phieuThuChiService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public PhieuThuChisController(IPhieuThuChiService phieuThuChiService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _phieuThuChiService = phieuThuChiService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //api get phan trang loai thu , chi
        [HttpGet]
        public async Task<IActionResult> GetLoaiThuChi([FromQuery]PhieuThuChiPaged val)
        {
            var res = await _phieuThuChiService.GetPhieuThuChiPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _phieuThuChiService.GetByIdPhieuThuChi(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        //api create
        [HttpPost]
        public async Task<IActionResult> Create(PhieuThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var phieuThuChi = await _phieuThuChiService.CreatePhieuThuChi(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<PhieuThuChiBasic>(phieuThuChi);
            return Ok(basic);
        }

        //api update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PhieuThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _phieuThuChiService.UpdatePhieuThuChi(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _phieuThuChiService.ActionConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _phieuThuChiService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        //api xóa
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _phieuThuChiService.Unlink(id);

            return NoContent();
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(PhieuThuChiDefaultGet val)
        {
            var res = new PhieuThuChiDisplay();
            res.Type = val.Type;
            res.CompanyId = CompanyId;
            return Ok(res);
        }
    }
}