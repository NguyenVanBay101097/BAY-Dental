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
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleOrderPromotionsController(IMapper mapper, ISaleOrderPromotionService orderPromotionService, IUnitOfWorkAsync unitOfWork)
        {
            _mapper = mapper;
            _orderPromotionService = orderPromotionService;
            _unitOfWork = unitOfWork;
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
            var stream = new MemoryStream();
            var data = await _orderPromotionService.GetHistoryPromotionResult(val);
            byte[] fileContent;


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Ngày sử dụng";
                worksheet.Cells[1, 2].Value = "Khách hàng";
                worksheet.Cells[1, 3].Value = "Số phiếu";
                worksheet.Cells[1, 4].Value = "Tiền điều trị";
                worksheet.Cells[1, 5].Value = "Giá trị khuyến mãi";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

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

                worksheet.Column(8).Style.Numberformat.Format = "@";
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
