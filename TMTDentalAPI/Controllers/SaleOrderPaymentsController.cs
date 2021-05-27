using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Utilities;
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
    public class SaleOrderPaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPaymentService _saleOrderPaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;

        public SaleOrderPaymentsController(IMapper mapper, ISaleOrderPaymentService saleOrderPaymentService, IUnitOfWorkAsync unitOfWork, IViewRenderService viewRenderService)
        {
            _mapper = mapper;
            _saleOrderPaymentService = saleOrderPaymentService;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaymentPaged val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistoryPaymentMethod([FromQuery] SaleOrderPaymentMethodFilter val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultPaymentMethodAsync(val);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            var saleOrderPayment = _saleOrderPaymentService.GetDisplay(id);

            _unitOfWork.Commit();

            return Ok(saleOrderPayment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleOrderPaymentSave val)
        {
            var saleOrderPayment = await _saleOrderPaymentService.CreateSaleOrderPayment(val);
            var basic = _mapper.Map<SaleOrderPaymentBasic>(saleOrderPayment);
            return Ok(basic);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionPayment(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionPayment(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportCustomerDebtExcelFile([FromQuery] SaleOrderPaymentMethodFilter val)
        {
            var stream = new MemoryStream();
            var data = await _saleOrderPaymentService.GetCustomerDebtExportExcel(val);
            byte[] fileContent;
            var sheetName = "Sổ công nợ khách hàng";


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "Ngày ghi nợ";
                worksheet.Cells[1, 2].Value = "Số phiếu";
                worksheet.Cells[1, 3].Value = "Nguồn";
                worksheet.Cells[1, 4].Value = "Công nợ";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.PaymentDate;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Orders.First().Name;
                    worksheet.Cells[row, 3].Value = item.PaymentName;
                    worksheet.Cells[row, 4].Value = item.PaymentAmount;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,###";
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

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _saleOrderPaymentService.GetPrint(id);
            if (res == null) return NotFound();
            var html = _viewRenderService.Render("SaleOrderPayment/Print", res);

            return Ok(new PrintData() { html = html });
        }
    }
}
