using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleCouponsController : BaseApiController
    {
        private readonly ISaleCouponService _couponService;
        private readonly IMapper _mapper;

        public SaleCouponsController(ISaleCouponService couponService,
            IMapper mapper)
        {
            _couponService = couponService;
            _mapper = mapper;
        }

        [HttpGet]
        [CheckAccess(Actions = "SaleCoupon.SaleCoupons.Read")]
        public async Task<IActionResult> Get([FromQuery]SaleCouponPaged val)
        {
            var result = await _couponService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "SaleCoupon.SaleCoupons.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var display = await _couponService.GetDisplay(id);
            if (display == null)
                return NotFound();

            return Ok(display);
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "SaleCoupon.SaleCoupons.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var program = await _couponService.GetByIdAsync(id);
            if (program == null)
                return NotFound();
            await _couponService.DeleteAsync(program);

            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCoupons.Read")]
        public async Task<IActionResult> ExportFile(SaleCouponPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            val.Offset = 0;
            var paged = await _couponService.GetPagedResultAsync(val);
            var sheetName = "Coupon";
            byte[] fileContent;

            var stateDict = new Dictionary<string, string>()
            {
                {"used","Đã sử dụng"},
                {"expired","Đã hết hạn"},
                {"reserved","Để dành riêng"},
                {"new","Có giá trị"},
            };
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Mã coupon";
                worksheet.Cells[1, 2].Value = "Ngày hết hạn";
                worksheet.Cells[1, 3].Value = "Tên chương trình";
                worksheet.Cells[1, 4].Value = "Trạng thái";
                var items = paged.Items.ToList();

                var row = 2;
                foreach (var item in items)
                {
                    worksheet.Cells[row, 1].Value = item.Code;

                    worksheet.Cells[row, 2].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = item.DateExpired;

                    worksheet.Cells[row, 3].Value = item.ProgramName;
                    worksheet.Cells[row, 4].Value = stateDict[item.State];
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