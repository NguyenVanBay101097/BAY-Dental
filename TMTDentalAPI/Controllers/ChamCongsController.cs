using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChamCongsController : BaseApiController
    {
        private readonly IChamCongService _chamCongService;
        private readonly IHrPayslipService _hrPayslipService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        public ChamCongsController(IHrPayslipService hrPayslipService, IEmployeeService employeeService, IChamCongService chamCongService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _chamCongService = chamCongService;
            _mapper = mapper;
            _hrPayslipService = hrPayslipService;
            _unitOfWork = unitOfWork;
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] ChamCongPaged val)
        {
            var res = await _chamCongService.GetPaged(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SyncToTimeKeeper(IEnumerable<ImportFileExcellChamCongModel> vals)
        {
            if (vals == null || !ModelState.IsValid)
                return BadRequest();

            var result = await _chamCongService.SyncChamCong(vals);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var chamcong = await _mapper.ProjectTo<ChamCongDisplay>(_chamCongService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();

            return Ok(chamcong);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> ExcelImportCreate(PartnerImportExcelViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var result = await _chamCongService.ImportExcel(val);
            if (result.Success)
                _unitOfWork.Commit();

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLastChamCong([FromQuery] employeePaged val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();

            var cc = await _chamCongService.GetLastChamCong(val);
            var res = _mapper.Map<ChamCongDisplay>(cc);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChamCongSave val)
        {
            var chamcong = _mapper.Map<ChamCong>(val);
            await _unitOfWork.BeginTransactionAsync();
            await _chamCongService.CreateAsync(chamcong);
            _unitOfWork.Commit();

            var display = _mapper.Map<ChamCongDisplay>(chamcong);
            return Ok(display);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ChamCongSave val)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();

            chamcong = _mapper.Map(val, chamcong);
            await _unitOfWork.BeginTransactionAsync();
            await _chamCongService.UpdateAsync(chamcong);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var chamcong = await _chamCongService.SearchQuery(x => x.Id == id).FirstOrDefaultAsync();
            if (chamcong == null)
                return NotFound();

            await _chamCongService.DeleteAsync(chamcong);
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGet(ChamCongDefaultGetPost val)
        {
            var res = await _chamCongService.DefaultGet(val);
            return Ok(res);
        }
    }
}
