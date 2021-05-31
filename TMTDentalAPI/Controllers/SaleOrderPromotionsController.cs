using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderPromotionsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPromotionService _orderPromotionService;
        private readonly ISaleCouponProgramService _programService;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleOrderPromotionsController(IMapper mapper, ISaleOrderPromotionService orderPromotionService, IUnitOfWorkAsync unitOfWork,
            ISaleCouponProgramService programService)
        {
            _mapper = mapper;
            _orderPromotionService = orderPromotionService;
            _unitOfWork = unitOfWork;
            _programService = programService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPromotionPaged val)
        {
            var result = await _orderPromotionService.GetPagedResultAsync(val);
            return Ok(result);
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistoryPromotionByCouponProgram([FromQuery] HistoryPromotionRequest val)
        {
            var result = await _orderPromotionService.GetHistoryPromotionResult(val);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemovePromotion(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _orderPromotionService.RemovePromotion(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelFile(HistoryPromotionRequest val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _orderPromotionService.GetHistoryPromotionResult(val);
            byte[] fileContent;

            var program = await _programService.GetByIdAsync(val.SaleCouponProgramId);
            if (program.DiscountApplyOn == "on_order")
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    worksheet.Cells[1, 1].Value = "Ngày sử dụng";
                    worksheet.Cells[1, 2].Value = "Khách hàng";
                    worksheet.Cells[1, 3].Value = "Số phiếu";
                    worksheet.Cells[1, 4].Value = "Tiền điều trị";
                    worksheet.Cells[1, 5].Value = "Giá trị khuyến mãi";

                    worksheet.Cells["A1:E1"].Style.Font.Bold = true;

                    var row = 2;
                    foreach (var item in data.Items)
                    {
                        worksheet.Cells[row, 1].Value = item.DatePromotion;
                        worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                        worksheet.Cells[row, 2].Value = item.PartnerName;
                        worksheet.Cells[row, 3].Value = item.SaleOrderName;
                        worksheet.Cells[row, 4].Value = item.Amount;
                        worksheet.Cells[row, 4].Style.Numberformat.Format = "#,###";
                        worksheet.Cells[row, 5].Value = item.AmountPromotion;
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "#,###";
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
            else
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    worksheet.Cells[1, 1].Value = "Ngày sử dụng";
                    worksheet.Cells[1, 2].Value = "Khách hàng";
                    worksheet.Cells[1, 3].Value = "Số phiếu";
                    worksheet.Cells[1, 4].Value = "Dịch vụ";
                    worksheet.Cells[1, 5].Value = "Tiền dịch vụ";
                    worksheet.Cells[1, 6].Value = "Giá trị khuyến mãi";

                    worksheet.Cells["A1:F1"].Style.Font.Bold = true;

                    var row = 2;
                    foreach (var item in data.Items)
                    {
                        worksheet.Cells[row, 1].Value = item.DatePromotion;
                        worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                        worksheet.Cells[row, 2].Value = item.PartnerName;
                        worksheet.Cells[row, 3].Value = item.SaleOrderName;
                        worksheet.Cells[row, 4].Value = item.SaleOrderLineName;
                        worksheet.Cells[row, 5].Value = item.Amount;
                        worksheet.Cells[row, 5].Style.Numberformat.Format = "#,###";
                        worksheet.Cells[row, 6].Value = item.AmountPromotion;
                        worksheet.Cells[row, 6].Style.Numberformat.Format = "#,###";
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
}
