using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMTDentalAPI.JobFilters;
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
        [CheckAccess(Actions = "Labo.LaboWarranty.Read")]
        public async Task<IActionResult> Get([FromQuery] LaboWarrantyPaged val)
        {
            var result = await _laboWarrantyService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboWarranty.Read")]
        public async Task<IActionResult> GetDefault(LaboWarrantyGetDefault val)
        {
            var res = await _laboWarrantyService.GetDefault(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Labo.LaboWarranty.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _laboWarrantyService.GetLaboWarrantyDisplay(id);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Labo.LaboWarranty.Create")]
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
        [CheckAccess(Actions = "Labo.LaboWarranty.Update")]
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
        [CheckAccess(Actions = "Labo.LaboWarranty.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboWarranty.Update")]
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
        [CheckAccess(Actions = "Labo.LaboWarranty.Cancel")]
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
        public async Task<IActionResult> CancelSendWarranty(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.CancelSendWarranty(ids);
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
        public async Task<IActionResult> CancelReceiptInspection(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.CancelReceiptInspection(ids);
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
        public async Task<IActionResult> CancelAssemblyWarranty(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _laboWarrantyService.CancelAssemblyWarranty(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Labo.LaboWarranty.Read")]
        public async Task<IActionResult> GetExcelFile([FromBody] LaboWarrantyPaged val)
        {
            var result = await _laboWarrantyService.GetExcelFile(val);
            var stream = new MemoryStream();
            byte[] fileContent;
            var dateToDate = "";
            if (val.DateReceiptFrom.HasValue && val.DateReceiptTo.HasValue)
                dateToDate = $"Từ ngày {val.DateReceiptFrom.Value.ToString("dd/MM/yyyy")} đến ngày {val.DateReceiptTo.Value.ToString("dd/MM/yyyy")}";

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("QuanLyBaoHanh");

                worksheet.Cells[1, 1, 1, 9].Value = "QUẢN LÝ BẢO HÀNH";
                worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
                worksheet.Cells[1, 1, 1, 9].Style.Font.Size = 14;
                worksheet.Cells[1, 1, 1, 9].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells[1, 1, 1, 9].Merge = true;
                worksheet.Cells[1, 1, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 1, 2, 9].Value = dateToDate;
                worksheet.Cells[2, 1, 2, 9].Merge = true;
                worksheet.Cells[2, 1, 2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells[4, 1].Value = "Ngày nhận bảo hành";
                worksheet.Cells[4, 2].Value = "Phiếu bảo hành";
                worksheet.Cells[4, 3].Value = "Phiếu Labo";
                worksheet.Cells[4, 4].Value = "Khách hàng";
                worksheet.Cells[4, 5].Value = "Nhà cung cấp";
                worksheet.Cells[4, 6].Value = "Gửi bảo hành";
                worksheet.Cells[4, 7].Value = "Nhận nghiệm thu";
                worksheet.Cells[4, 8].Value = "Lắp bảo hành";
                worksheet.Cells[4, 9].Value = "Trạng thái";

                worksheet.Cells["A4:I4"].Style.Font.Bold = true;
                worksheet.Cells["A4:I4"].Style.Font.Size = 14;
                worksheet.Cells["A4:I4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A4:I4"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A4:I4"].Style.Font.Color.SetColor(Color.White);
                for (int i = 1; i <= 9; i++)
                {
                    worksheet.Cells[4, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }
                var row = 5;
                foreach (var item in result)
                {
                    worksheet.Cells[row, 1].Value = item.DateReceiptWarranty;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.LaboOrderName;
                    worksheet.Cells[row, 4].Value = item.CustomerName;
                    worksheet.Cells[row, 5].Value = item.SupplierName;
                    worksheet.Cells[row, 6].Value = item.DateSendWarranty;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 7].Value = item.DateReceiptInspection;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 8].Value = item.DateAssemblyWarranty;
                    worksheet.Cells[row, 8].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 9].Value = GetState(item.State);
                    for (int i = 1; i <= 9; i++)
                    {
                        worksheet.Cells[row, i].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
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

        private string GetState(string state)
        {
            switch (state)
            {
                case "new":
                    return "Mới";
                case "sent":
                    return "Đã gửi";
                case "received":
                    return "Đã nhận";
                case "assembled":
                    return "Đã lắp";
                default:
                    return "Nháp";
            }
        }
    }
}
