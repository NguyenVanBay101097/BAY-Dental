using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
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
        private readonly IViewRenderService _viewRenderService;
        public PhieuThuChisController(IPhieuThuChiService phieuThuChiService, IMapper mapper, IUnitOfWorkAsync unitOfWork,
            IViewRenderService viewRenderService)
        {
            _phieuThuChiService = phieuThuChiService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
        }

        //api get phan trang loai thu , chi
        [HttpGet]
        [CheckAccess(Actions = "Account.PhieuThuChi.Read")]
        public async Task<IActionResult> GetLoaiThuChi([FromQuery] PhieuThuChiPaged val)
        {
            var res = await _phieuThuChiService.GetPhieuThuChiPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Account.PhieuThuChi.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _phieuThuChiService.GetByIdPhieuThuChi(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Account.PhieuThuChi.Read")]
        public async Task<IActionResult> ReportPhieuThuChi([FromQuery] PhieuThuChiSearch val)
        {
            if (val == null || !ModelState.IsValid)
                return BadRequest();
            var res = await _phieuThuChiService.ReportPhieuThuChi(val);
            return Ok(res);
        }

        //api create
        [HttpPost]
        [CheckAccess(Actions = "Account.PhieuThuChi.Create")]
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
        [CheckAccess(Actions = "Account.PhieuThuChi.Update")]
        public async Task<IActionResult> Update(Guid id, PhieuThuChiSave val)
        {
            await _unitOfWork.BeginTransactionAsync();

            await _phieuThuChiService.UpdatePhieuThuChi(id, val);

            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.PhieuThuChi.Update")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _phieuThuChiService.ActionConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Account.PhieuThuChi.Update")]
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
        [CheckAccess(Actions = "Account.PhieuThuChi.Delete")]
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

        [AllowAnonymous]
        [HttpGet("{id}/[action]")]
        //[CheckAccess(Actions = "Account.PhieuThuChi.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var phieu = await _mapper.ProjectTo<PhieuThuChiPrintVM>(_phieuThuChiService.SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (phieu == null)
                return NotFound();

            phieu.AmountText = AmountToText.amount_to_text(phieu.Amount);

            var html = _viewRenderService.Render("PhieuThuChi/Print", phieu);

            return Ok(new PrintData() { html = html });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] PhieuThuChiPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var services = await _phieuThuChiService.GetExportExcel(val);
            var sheetName = "Phiếu " + val.Type;

            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Số phiếu";
                worksheet.Cells[1, 3].Value = "Phương thức thanh toán";
                worksheet.Cells[1, 4].Value = "Loại " + val.Type;
                worksheet.Cells[1, 5].Value = "Số tiền";
                if (val.Type == "thu")
                {
                    worksheet.Cells[1, 6].Value = "Người nộp tiền";
                } else
                {
                    worksheet.Cells[1, 6].Value = "Người nhận tiền";
                }
                worksheet.Cells[1, 7].Value = "Nội dung";
                worksheet.Cells[1, 8].Value = "Trạng thái";

                for (int row = 2; row < services.Count() + 2; row++)
                {
                    var item = services.ToList()[row - 2];

                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.JournalName;
                    worksheet.Cells[row, 4].Value = item.LoaiThuChiName;
                    worksheet.Cells[row, 5].Value = item.Amount;
                    worksheet.Cells[row, 6].Value = item.PayerReceiver;
                    worksheet.Cells[row, 7].Value = item.Reason;
                    worksheet.Cells[row, 8].Value = item.State;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }
    }
}