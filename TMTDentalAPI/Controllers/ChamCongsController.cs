using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Spreadsheet;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChamCongsController : BaseApiController
    {
        private readonly IChamCongService _chamCongService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ChamCongsController(IChamCongService chamCongService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _chamCongService = chamCongService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async  Task<IActionResult> Get([FromQuery] employeePaged val)
        {
            var res = await _chamCongService.GetByEmployeePaged(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var chamcong = await _chamCongService.SearchQuery(x=>x.Id == id).Include(x=>x.Employee).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();
            var res = _mapper.Map<ChamCongDisplay>(chamcong);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChamCongSave chamCongSave)
        {
            var lst = new List<ChamCong>();
            lst.Add(_mapper.Map<ChamCong>(chamCongSave));
            await _chamCongService.CreateListChamcongs(lst);
            return Ok();
        }

        [HttpPost("CreateList")]
        public async Task<IActionResult> CreateList(IEnumerable<ChamCongSave> chamCongSaves)
        {
            await _chamCongService.CreateListChamcongs(_mapper.Map<IEnumerable<ChamCong>>(chamCongSaves));
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id , ChamCongSave chamCongSave)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
            {
                return NotFound();
            }
            chamcong = _mapper.Map(chamCongSave,chamcong);
            chamcong.CompanyId = _chamCongService.GetCurrentCompanyId();
            await _chamCongService.UpdateAsync(chamcong);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
            {
                return NotFound();
            }
            await _chamCongService.DeleteAsync(chamcong);
            return NoContent();
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByEmployee(Guid id, DateTime date)
        {
            var res = await _chamCongService.GetByEmployeeId(id,date);
            return Ok(res);
        }
    }
}
