using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LoaiThuChiController : BaseApiController
    {
        private readonly ILoaiThuChiService _loaiThuChiService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public LoaiThuChiController(ILoaiThuChiService loaiThuChiService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _loaiThuChiService = loaiThuChiService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //api get phan trang loai thu , chi
        [HttpGet]
        [CheckAccess(Actions = "Account.LoaiThuChi.Read")]
        public async Task<IActionResult> GetLoaiThuChi([FromQuery]LoaiThuChiPaged val)
        {
            var res = await _loaiThuChiService.GetThuChiPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Account.LoaiThuChi.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _loaiThuChiService.GetByIdThuChi(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost("[action]")]
        public IActionResult DefaultGet(LoaiThuChiDefault val)
        {
            var res = _loaiThuChiService.DefaultGet(val);
            return Ok(res);
        }

        //api create
        [HttpPost]
        [CheckAccess(Actions = "Account.LoaiThuChi.Create")]
        public async Task<IActionResult> Create(LoaiThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            var loaithuchi = await _loaiThuChiService.CreateLoaiThuChi(val);

            _unitOfWork.Commit();

            var basic = _mapper.Map<LoaiThuChiBasic>(loaithuchi);
            return Ok(basic);
        }

        //api update
        [HttpPut("{id}")]
        [CheckAccess(Actions = "Account.LoaiThuChi.Update")]
        public async Task<IActionResult> Update(Guid id, LoaiThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _loaiThuChiService.UpdateLoaiThuChi(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        //api xóa
        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Account.LoaiThuChi.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {         
            await _loaiThuChiService.RemoveLoaiThuChi(id);

            return NoContent();
        }
    }
}