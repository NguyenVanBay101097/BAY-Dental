using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
        private readonly IAccountJournalService _journalService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public PhieuThuChisController(IPhieuThuChiService phieuThuChiService, IMapper mapper, IUnitOfWorkAsync unitOfWork,
            IViewRenderService viewRenderService, IAccountJournalService journalService,
            IPrintTemplateConfigService printTemplateConfigService,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService)
        {
            _phieuThuChiService = phieuThuChiService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
            _journalService = journalService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
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
        public async Task<IActionResult> DefaultGet(PhieuThuChiDefaultGet val)
        {
            var res = new PhieuThuChiDisplay();
            res.Date = DateTime.Now;
            res.Type = val.Type;
            res.CompanyId = CompanyId;
            var journal = await _journalService.SearchQuery(x => x.CompanyId == CompanyId && x.Type == "cash").FirstOrDefaultAsync();
            res.Journal = _mapper.Map<AccountJournalSimple>(journal);
            res.IsAccounting = false;
            return Ok(res);
        }

        [HttpGet("{id}/Print")]
        //[CheckAccess(Actions = "Account.PhieuThuChi.Read")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _phieuThuChiService.GetByIdAsync(id);
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == (res.Type == "thu" ? "tmp_phieu_thu" : "tmp_phieu_chi") && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>(res.Type == "thu" ? "base.print_template_phieu_thu" : "base.print_template_phieu_chi");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpGet("{id}/Print2")]
        //[CheckAccess(Actions = "Account.PhieuThuChi.Read")]
        public async Task<IActionResult> GetPrint2(Guid id)
        {
            var res = await _phieuThuChiService.GetByIdAsync(id);
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == (res.AccountType == "customer_debt" ? "tmp_customer_debt" : "tmp_agent_commission") && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>(res.AccountType == "customer_debt" ? "base.print_template_customer_debt" : "base.print_template_agent_commission");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] PhieuThuChiPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var services = await _phieuThuChiService.GetExportExcel(val);
            var sheetName = val.Type == "thu" ? "Phiếu thu" : "Phiếu chi";

            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày";
                worksheet.Cells[1, 2].Value = "Số phiếu";
                worksheet.Cells[1, 3].Value = "Phương thức";
                worksheet.Cells[1, 4].Value = val.Type == "thu" ? "Loại thu" : "Loại chi";
                worksheet.Cells[1, 5].Value = "Số tiền";
                worksheet.Cells[1, 6].Value = val.Type == "thu" ? "Người nộp tiền" : "Người nhận tiền";
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
                    worksheet.Cells[row, 6].Value = item.PartnerDisplayName;
                    worksheet.Cells[row, 7].Value = item.Reason;
                    worksheet.Cells[row, 8].Value = item.StateDisplay;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcelCommissionAgentFile([FromQuery] PhieuThuChiPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var data = await _phieuThuChiService.GetPhieuThuChiPagedResultAsync(val);
            var sheetName = "LichSuChiHoaHong";

            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1:F1"].Value = $"Người giới thiệu: {data.Items.ToArray()[0].PartnerName}";
                worksheet.Cells["A1:F1"].Style.Font.Size = 14;
                //worksheet.Cells["A1:F1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:F1"].Merge = true;
                worksheet.Cells["A1:F1"].Style.Font.Bold = true;

                worksheet.Cells[3, 1].Value = "Ngày";
                worksheet.Cells[3, 2].Value = "Số phiếu";
                worksheet.Cells[3, 3].Value = "Số tiền";
                worksheet.Cells[3, 4].Value = "Phương thức";
                worksheet.Cells[3, 5].Value = "Nội dung";
                worksheet.Cells[3, 6].Value = "Trạng thái";
                worksheet.Cells[3, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[3, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells["A3:F3"].Style.Font.Bold = true;
                worksheet.Cells["A3:F3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:F3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A3:F3"].Style.Font.Color.SetColor(Color.White);
         

                var row = 4;
                foreach (var item in data.Items)
                {                
                    worksheet.Cells[row, 1].Value = item.Date;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.Amount;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,###";
                    worksheet.Cells[row, 4].Value = item.JournalName;                  
                    worksheet.Cells[row, 5].Value = item.Reason;
                    worksheet.Cells[row, 6].Value = item.State == "posted" ? "Đã thanh toán" : "Hủy";
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row++;
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