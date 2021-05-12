using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleCouponProgramsController : BaseApiController
    {
        private readonly ISaleCouponProgramService _programService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IMapper _mapper;

        public SaleCouponProgramsController(ISaleCouponProgramService programService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _programService = programService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Read")]
        public async Task<IActionResult> Get([FromQuery] SaleCouponProgramPaged val)
        {
            var result = await _programService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var display = await _programService.GetDisplay(id);
            if (display == null)
                return NotFound();

            return Ok(display);
        }


        [HttpGet("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Read")]
        public async Task<IActionResult> GetPromotionBySaleOrder()
        {
            var result = await _programService.GetPromotionBySaleOrder();
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Read")]
        public async Task<IActionResult> GetPromotionBySaleOrderLine([FromQuery] Guid productId)
        {
            var result = await _programService.GetPromotionBySaleOrderLine(productId);
            return Ok(result);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Read")]
        public async Task<IActionResult> GetPromotionUsageCode([FromQuery] string code, Guid? productId)
        {
            var result = await _programService.GetPromotionDisplayUsageCode(code, productId);
            return Ok(result);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Read")]
        public async Task<IActionResult> GetAmountTotalUsagePromotion(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var amountTotal = await _programService.GetAmountTotal(id);
            _unitOfWork.Commit();

            return Ok(amountTotal);
        }

        [HttpPost]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Create")]
        public async Task<IActionResult> Create(SaleCouponProgramSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            var program = await _programService.CreateProgram(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleCouponProgramSimple>(program);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Update")]
        public async Task<IActionResult> Update(Guid id, SaleCouponProgramSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _programService.UpdateProgram(id, val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _programService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCoupons.Create")]
        public async Task<IActionResult> GenerateCoupons(SaleCouponProgramGenerateCoupons val)
        {
            await _programService.GenerateCoupons(val);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _programService.Unlink(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Update")]
        public async Task<IActionResult> ToggleActive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _programService.ToggleActive(ids);
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Update")]
        public async Task<IActionResult> ActionArchive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _programService.ActionArchive(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "SaleCoupon.SaleCouponProgram.Update")]
        public async Task<IActionResult> ActionUnArchive(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _programService.ActionUnArchive(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcelFile(HistoryPromotionRequest val)
        {
            var stream = new MemoryStream();
            var data = await _programService.GetExcel(val);
            byte[] fileContent;

            //var gender_dict = new Dictionary<string, string>()
            //{
            //    { "male", "Nam" },
            //    { "female", "Nữ" },
            //    { "other", "Khác" }
            //};

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
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.DatePromotion;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 2].Value = item.PartnerName;
                    worksheet.Cells[row, 3].Value = item.SaleOrderName;
                    worksheet.Cells[row, 4].Value = item.Amount;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#.#";
                    worksheet.Cells[row, 5].Value = item.AmountPromotion;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#.#";
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