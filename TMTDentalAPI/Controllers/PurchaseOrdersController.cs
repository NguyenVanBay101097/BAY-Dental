
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
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
    public class PurchaseOrdersController : BaseApiController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IPrintTemplateConfigService printTemplateConfigService, IPrintTemplateService printTemplateService, IIRModelDataService modelDataService)
        {
            _purchaseOrderService = purchaseOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
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
                    worksheet.Cells[row, 7].Style.Numberformat.Format = (item.AmountTotal ?? 0) > 0 ? "#,###" : "0";
                    worksheet.Cells[row, 6].Value = item.State != "draft" ? ((item.AmountTotal ?? 0) - (item.AmountResidual ?? 0)) : 0;
                    worksheet.Cells[row, 6].Style.Numberformat.Format = ((item.AmountTotal ?? 0) - (item.AmountResidual ?? 0)) > 0 && item.State != "draft" ? "#,###" : "0";
                    worksheet.Cells[row, 7].Value = item.AmountResidual ?? 0;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = (item.AmountResidual ?? 0) > 0 ? "#,###" : "0";
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

        [HttpGet("{id}/Print")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            var res = await _purchaseOrderService.GetByIdAsync(id);
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == (res.Type == "order" ? "tmp_purchase_order" : "tmp_purchase_refund") && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>(res.Type == "order" ? "base.print_template_purchase_order" : "base.print_template_purchase_refund");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);
            return Ok(new PrintData() { html = result });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateOrderXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\purchase_order_order.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "stock" || x.Module == "account" || x.Module == "product")).ToListAsync();// các irmodel cần thiết
            var entities = await _purchaseOrderService.SearchQuery(x => x.Type == "order").Include(x => x.OrderLines).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<PurchaseOrderXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<PurchaseOrderXmlSampleDataRecord>(entity);

                item.Id = $@"sample.purchase_order_order_{entities.IndexOf(entity) + 1}";
                item.DateRound = (int)(dateToData.Date - entity.DateOrder.Date).TotalDays;
                var irmodelDataPartner = listIrModelData.FirstOrDefault(x => x.ResId == entity.PartnerId.ToString());
                item.PartnerId = irmodelDataPartner == null ? "" : irmodelDataPartner?.Module + "." + irmodelDataPartner?.Name;
                var irmodelDataPickingType = listIrModelData.FirstOrDefault(x => x.ResId == entity.PickingTypeId.ToString());
                item.PickingTypeId = irmodelDataPickingType == null ? "" : irmodelDataPickingType.Module + "." + irmodelDataPickingType?.Name;
                var irmodelDataJournal = listIrModelData.FirstOrDefault(x => x.ResId == entity.JournalId.ToString());
                item.JournalId = irmodelDataJournal == null ? "" : irmodelDataJournal.Module + "." + irmodelDataJournal?.Name;
                //add lines
                foreach (var lineEntity in entity.OrderLines)
                {
                    var itemLine = _mapper.Map<PurchaseOrderLineXmlSampleDataRecord>(lineEntity);
                    var irmodelDataProductUom = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductUOMId.ToString());
                    itemLine.ProductUOMId = irmodelDataProductUom?.Module + "." + irmodelDataProductUom?.Name;
                    var irmodelDataProduct = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductId.ToString());
                    itemLine.ProductId = irmodelDataProduct?.Module + "." + irmodelDataProduct?.Name;
                    var irmodelDataPartnerLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.PartnerId.ToString());
                    item.OrderLines.Add(itemLine);
                }
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "purchase.order",
                    ResId = entity.Id.ToString(),
                    Name = $"purchase_order_order_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateRefundXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\purchase_order_refund.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "stock" || x.Module == "account" || x.Module == "product")).ToListAsync();// các irmodel cần thiết
            var entities = await _purchaseOrderService.SearchQuery(x => x.Type == "refund" && x.DateOrder.Date <= dateToData.Date).Include(x => x.OrderLines).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<PurchaseOrderXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<PurchaseOrderXmlSampleDataRecord>(entity);

                item.Id = $@"sample.purchase_order_refund_{entities.IndexOf(entity) + 1}";
                item.DateRound = (int)(dateToData.Date - entity.DateOrder.Date).TotalDays;
                var irmodelDataPartner = listIrModelData.FirstOrDefault(x => x.ResId == entity.PartnerId.ToString());
                item.PartnerId = irmodelDataPartner == null ? "" : irmodelDataPartner?.Module + "." + irmodelDataPartner?.Name;
                var irmodelDataPickingType = listIrModelData.FirstOrDefault(x => x.ResId == entity.PickingTypeId.ToString());
                item.PickingTypeId = irmodelDataPickingType == null ? "" : irmodelDataPickingType.Module + "." + irmodelDataPickingType?.Name;
                var irmodelDataJournal = listIrModelData.FirstOrDefault(x => x.ResId == entity.JournalId.ToString());
                item.JournalId = irmodelDataJournal == null ? "" : irmodelDataJournal.Module + "." + irmodelDataJournal?.Name;
                //add lines
                foreach (var lineEntity in entity.OrderLines)
                {
                    var itemLine = _mapper.Map<PurchaseOrderLineXmlSampleDataRecord>(lineEntity);
                    var irmodelDataProductUom = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductUOMId.ToString());
                    itemLine.ProductUOMId = irmodelDataProductUom?.Module + "." + irmodelDataProductUom?.Name;
                    var irmodelDataProduct = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductId.ToString());
                    itemLine.ProductId = irmodelDataProduct?.Module + "." + irmodelDataProduct?.Name;
                    var irmodelDataPartnerLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.PartnerId.ToString());
                    item.OrderLines.Add(itemLine);
                }
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "purchase.order",
                    ResId = entity.Id.ToString(),
                    Name = $"purchase_order_refund_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}