
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
    public class PurchaseOrdersController : BaseApiController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork)
        {
            _purchaseOrderService = purchaseOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [CheckAccess(Actions = "Purchase.Order.Read")]
        public async Task<IActionResult> Get([FromQuery] PurchaseOrderPaged val)
        {
            var result = await _purchaseOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Purchase.Order.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _purchaseOrderService.GetPurchaseDisplay(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Purchase.Order.Read")]
        public async Task<IActionResult> GetRefundByOrder(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _purchaseOrderService.GetRefundByOrder(id);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Purchase.Order.Create")]
        public async Task<IActionResult> Create(PurchaseOrderSave val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var purchase = await _purchaseOrderService.CreatePurchaseOrder(val);
            _unitOfWork.Commit();
            var basic = _mapper.Map<PurchaseOrderBasic>(purchase);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Purchase.Order.Update")]
        public async Task<IActionResult> Update(Guid id, PurchaseOrderSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.UpdatePurchaseOrder(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Purchase.Order.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _purchaseOrderService.Unlink(new List<Guid>() { id });
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> DefaultGetAsync(PurchaseOrderDefaultGet val)
        {
            var res = await _purchaseOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Purchase.Order.Update")]
        public async Task<IActionResult> ButtonConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.ButtonConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Purchase.Order.Cancel")]
        public async Task<IActionResult> ButtonCancel(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.ButtonCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] PurchaseOrderPaged val)
        {
            var stream = new MemoryStream();
            val.Limit = int.MaxValue;
            var data = await _purchaseOrderService.GetPagedResultAsync(val);
            byte[] fileContent;
            var sheetName = val.Type == "order" ? "Mua-hang" : "Tra-hang";


            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Số phiếu";
                worksheet.Cells[1, 3].Value = "Nhà cung cấp";
                worksheet.Cells[1, 4].Value = val.Type == "order" ? "Ngày mua hàng" : "Ngày trả hàng";
                worksheet.Cells[1, 5].Value = "Tổng tiền";
                worksheet.Cells[1, 6].Value = val.Type == "order" ? "Đã thanh toán" : "Đã nhận hoàn";
                worksheet.Cells[1, 7].Value = "Còn nợ";
                worksheet.Cells[1, 8].Value = "Trạng thái";

                worksheet.Cells["A1:P1"].Style.Font.Bold = true;

                var row = 2;
                var index = 1;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = index;
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.PartnerName;
                    worksheet.Cells[row, 4].Value = item.DateOrder;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 5].Value = item.AmountTotal;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,###";
                    worksheet.Cells[row, 6].Value = item.State != "draft" ? ((item.AmountTotal ?? 0) - (item.AmountResidual ?? 0)) : 0000;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = ((item.AmountTotal ?? 0) - (item.AmountResidual ?? 0)) > 0 ? "#,###" : "0";
                    worksheet.Cells[row, 7].Value = item.AmountResidual ;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = item.AmountResidual > 0 ? "#,###" : "0";
                    worksheet.Cells[row, 8].Value = item.State == "draft" ? "Nháp" : (item.State == "purchase" ? "Đơn hàng" : "Hoàn thành");

                    row++;
                    index++;
                }

                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Purchase.Order.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _purchaseOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }
    }
}