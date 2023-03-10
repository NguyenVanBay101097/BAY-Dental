using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DinkToPdf;
using DinkToPdf.Contracts;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class SaleOrdersController : BaseApiController
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IDotKhamService _dotKhamService;
        private readonly ICardCardService _cardService;
        private readonly IProductPricelistService _pricelistService;
        private readonly ISaleOrderLineService _saleLineService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IViewToStringRenderService _viewToStringRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;
        private readonly IIrAttachmentService _irAttachmentService;
        private IConverter _converter;
        private readonly ISaleOrderPromotionService _saleOrderPromotionService;

        public SaleOrdersController(ISaleOrderService saleOrderService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IDotKhamService dotKhamService,
            ICardCardService cardService, IProductPricelistService pricelistService,
            ISaleOrderLineService saleLineService, IViewRenderService viewRenderService, IConverter converter,
            IIrAttachmentService irAttachmentService,
        IViewToStringRenderService viewToStringRenderService, IPrintTemplateConfigService printTemplateConfigService, IPrintTemplateService printTemplateService, IIRModelDataService modelDataService,
        ISaleOrderPromotionService saleOrderPromotionService)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dotKhamService = dotKhamService;
            _cardService = cardService;
            _pricelistService = pricelistService;
            _saleLineService = saleLineService;
            _viewRenderService = viewRenderService;
            _viewToStringRenderService = viewToStringRenderService;
            _converter = converter;
            _printTemplateConfigService = printTemplateConfigService;
            _modelDataService = modelDataService;
            _printTemplateService = printTemplateService;
            _irAttachmentService = irAttachmentService;
            _saleOrderPromotionService = saleOrderPromotionService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaged val)
        {
            var result = await _saleOrderService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetBasic(Guid id)
        {
            var order = await _saleOrderService.SearchQuery(x => x.Id == id)
                .Include(x => x.Partner)
                .Include(x => x.Doctor)
                .FirstOrDefaultAsync();

            return Ok(_mapper.Map<SaleOrderBasic>(order));
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetSaleOrderForSms([FromQuery] SaleOrderPaged val)
        {
            var result = await _saleOrderService.GetSaleOrderForSms(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _saleOrderService.GetSaleOrderForDisplayAsync(id);
            if (res == null)
                return NotFound();

            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetCountSaleOrder(GetCountSaleOrderFilter val)
        {
            var res = await _saleOrderService.GetCountSaleOrder(val);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Basic.SaleOrder.Create")]
        public async Task<IActionResult> Create(SaleOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.CreateOrderAsync(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> Update(Guid id, SaleOrderSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.UpdateOrderAsync(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Basic.SaleOrder.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var saleOrder = await _saleOrderService.GetSaleOrderByIdAsync(id);
            if (saleOrder == null)
                return NotFound();
            await _saleOrderService.UnlinkSaleOrderAsync(saleOrder);

            return NoContent();
        }

        [HttpGet("DefaultGet")]
        public async Task<IActionResult> DefaultGet([FromQuery] SaleOrderDefaultGet val)
        {
            var res = await _saleOrderService.DefaultGet(val);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> CheckPromotion(Guid id)
        {
            //Kiem tra co chuong trinh khuyen mai nao co the ap dung cho don hang nay khong?
            var res = await _saleOrderService.CheckHasPromotionCanApply(id);
            return Ok(res);
        }

        [HttpPost("DefaultGetInvoice")]
        public IActionResult DefaultGetInvoice(List<Guid> ids)
        {
            var res = _saleOrderService.DefaultGetInvoice(ids);
            return Ok(res);
        }

        [HttpPost("DefaultLineGet")]
        public async Task<IActionResult> DefaultLineGet(SaleOrderLineDefaultGet val)
        {
            var res = await _saleOrderService.DefaultLineGet(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Cancel")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update,Basic.SaleOrder.Create")]
        public async Task<IActionResult> ActionConvertToOrder(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.ActionConvertToOrder(id);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderBasic>(order);
            return Ok(basic);
        }

        [HttpPost("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionInvoiceCreateV2(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionInvoiceCreateV2(id);
            _unitOfWork.Commit();
            return NoContent();
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyCouponOnOrder(ApplyPromotionUsageCode val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _saleOrderService.ApplyCoupon(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.DiscountOrder")]
        public async Task<IActionResult> ApplyDiscountOnOrder(ApplyDiscountViewModel val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyDiscountOnOrder(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyPromotion(ApplyPromotionRequest val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyPromotionOnOrder(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.AccountPayment.Read")]
        public async Task<IActionResult> GetPayments(Guid id)
        {
            var res = await _saleOrderService._GetPaymentInfoJson(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Delete")]
        public async Task<IActionResult> Unlink(IEnumerable<Guid> ids)
        {
            if (ids == null || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.Unlink(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> OnChangePartner(SaleOrderOnChangePartner val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var res = new SaleOrderOnChangePartnerResult();
            if (val.PartnerId.HasValue)
            {
                //tìm bảng giá mặc định
                var pricelist = await _pricelistService.SearchQuery(x => !x.CompanyId.HasValue, orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
                if (pricelist == null)
                {
                    var companyId = CompanyId;
                    pricelist = await _pricelistService.SearchQuery(x => x.CompanyId == companyId, orderBy: x => x.OrderBy(s => s.Sequence)).FirstOrDefaultAsync();
                }

                if (pricelist != null)
                    res.Pricelist = _mapper.Map<ProductPricelistBasic>(pricelist);

                var card = await _cardService.GetValidCard(val.PartnerId.Value);
                if (card != null && card.Type.PricelistId.HasValue)
                    res.Pricelist = await _pricelistService.GetBasic(card.Type.PricelistId.Value);
            }

            return Ok(res);
        }

        [HttpPost("Print")]
        [CheckAccess(Actions = "Basic.SaleOrder.Print")]
        public async Task<IActionResult> GetPrint(SaleOrderPrint val)
        {

            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_sale_order" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_sale_order");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            //lấy object đầy đủ
            var saleOrders = await _saleOrderService.GetPrintTemplate(new List<Guid>() { val.Id });
            var attachments = await _irAttachmentService.SearchQuery(x => val.AttachmentIds.Contains(x.Id)).ToListAsync();
            foreach (var item in saleOrders)
            {
                item.IrAttachments = _mapper.Map<IEnumerable<SaleOrderOrderIrAttachmentPrintVM>>(attachments);
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, saleOrders);

            return Ok(new PrintData() { html = result });
        }

        private async Task SaveOrderLines(SaleOrderSave val, SaleOrder order)
        {
            var existLines = order.OrderLines.ToList();
            var lineToRemoves = new List<SaleOrderLine>();
            foreach (var existLine in existLines)
            {
                bool found = false;
                foreach (var item in val.OrderLines)
                {
                    if (item.Id == existLine.Id)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lineToRemoves.Add(existLine);
            }

            if (lineToRemoves.Any())
                await _saleLineService.Unlink(lineToRemoves.Select(x => x.Id).ToList());

            //Cập nhật sequence cho tất cả các line của val
            int sequence = 0;
            foreach (var line in val.OrderLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var saleLine = _mapper.Map<SaleOrderLine>(line);
                    saleLine.Sequence = sequence++;
                    foreach (var toothId in line.ToothIds)
                    {
                        saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                        {
                            ToothId = toothId
                        });
                    }
                    order.OrderLines.Add(saleLine);
                }
                else
                {
                    var saleLine = order.OrderLines.SingleOrDefault(c => c.Id == line.Id);
                    if (saleLine != null)
                    {
                        _mapper.Map(line, saleLine);
                        saleLine.Sequence = sequence++;
                        saleLine.SaleOrderLineToothRels.Clear();
                        foreach (var toothId in line.ToothIds)
                        {
                            saleLine.SaleOrderLineToothRels.Add(new SaleOrderLineToothRel
                            {
                                ToothId = toothId
                            });
                        }
                    }
                }
            }
        }


        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionConfirm(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionConfirm(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionDone(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ActionUnlock(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ActionUnlock(ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/GetDotKhamList")]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> GetDotKhamList(Guid id)
        {
            var res = await _dotKhamService.GetDotKhamBasicsForSaleOrder(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Update")]
        public async Task<IActionResult> ApplyServiceCards(SaleOrderApplyServiceCards val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyServiceCards(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyDiscountDefault(ApplyDiscountViewModel val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderService.ApplyDiscountDefault(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Create")]
        public async Task<IActionResult> CreateFastSaleOrder(FastSaleOrderSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var res = await _saleOrderService.CreateFastSaleOrder(val);
            _unitOfWork.Commit();
            return Ok(res);
        }


        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetServiceBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetServiceBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetTreatmentBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetTreatmentBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetLaboBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetLaboBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetSaleOrderPaymentBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetSaleOrderPaymentBySaleOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetDefaultInsurancePaymentBySaleOrderId(Guid id)
        {
            var res = await _saleOrderService.GetDefaultInsurancePaymentByOrderId(id);
            return Ok(res);
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetSaleProductionBySaleOrderId(Guid id)
        {
            var saleProductions = await _saleOrderService.GetSaleProductionBySaleOrderId(id);
            var res = _mapper.Map<IEnumerable<SaleProductionDisplay>>(saleProductions);
            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetPrintSaleOrder(Guid id)
        {
            var phieu = await _saleOrderService.GetPrint(id);
            if (phieu == null)
                return NotFound();

            //var html = await _viewToStringRenderService.RenderViewAsync("SaleOrder/Print", phieu);
            var html = _viewRenderService.Render("SaleOrder/Print", phieu);

            return Ok(new PrintData() { html = html });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ToSurvey([FromBody] SaleOrderToSurveyFilter val)
        {
            var paged = await _saleOrderService.GetToSurveyPagedAsync(val);
            return Ok(paged);
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> GetLineForProductRequest(Guid id)
        {
            var res = await _saleOrderService.GetLineForProductRequest(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetRevenueReport(SaleOrderRevenueReportPaged val) // dự kiến doanh thu
        {
            var res = await _saleOrderService.GetRevenueReport(val);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetRevenueSumTotal(GetRevenueSumTotalReq val) //Tổng dự kiến doanh thu
        {
            var res = await _saleOrderService.GetRevenueSumTotal(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> ExportExcelFile([FromQuery] SaleOrderPaged val)
        {
            var stream = new MemoryStream();
            var data = await _saleOrderService.GetExcel(val);
            byte[] fileContent;
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "Ngày lập phiếu";
                worksheet.Cells[1, 2].Value = "Số phiếu";
                worksheet.Cells[1, 3].Value = "Khách hàng";
                worksheet.Cells[1, 4].Value = "Tiền điều trị";
                worksheet.Cells[1, 5].Value = "Thanh toán";
                worksheet.Cells[1, 6].Value = "Còn lại";
                worksheet.Cells["A1:F1"].Style.Font.Bold = true;

                var row = 2;
                var rowF = row;
                var rowE = row;
                foreach (var itemParent in data)
                {
                    worksheet.Cells[row, 1].Value = itemParent.DateOrder;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = itemParent.Name;
                    worksheet.Cells[row, 3].Value = itemParent.PartnerName;
                    worksheet.Cells[row, 4].Value = itemParent.AmountTotal;
                    worksheet.Cells[row, 5].Value = itemParent.TotalPaid;
                    worksheet.Cells[row, 6].Value = itemParent.Residual;

                    row++;
                    rowF = row;
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Dịch vụ";
                    worksheet.Cells[row, 3].Value = "Số lượng";
                    worksheet.Cells[row, 4].Value = "Thành tiền";
                    worksheet.Cells[row, 5].Value = "Thanh toán";
                    worksheet.Cells[row, 6].Value = "Còn lại";
                    row++;
                    foreach (var itemChild in itemParent.SaleOrderLineDisplays)
                    {
                        worksheet.Cells[row, 1].Value = "";
                        worksheet.Cells[row, 2].Value = itemChild.Name;
                        worksheet.Cells[row, 3].Value = itemChild.ProductUOMQty;
                        worksheet.Cells[row, 4].Value = itemChild.PriceSubTotal;
                        worksheet.Cells[row, 5].Value = itemChild.AmountPaid;
                        worksheet.Cells[row, 6].Value = itemChild.AmountResidual;

                        row++;

                    }
                    rowE = row - 1;
                    worksheet.Cells[$"B{rowF}:F{rowF}"].Style.Font.Bold = true;
                    worksheet.Cells[$"A{rowF}:A{rowE}"].Merge = true;

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
        public async Task<IActionResult> GetRevenueReportPdf([FromQuery] SaleOrderRevenueReportPaged val)
        {
            var data = await _saleOrderService.GetRevenueReportPrint(val);
            var html = _viewRenderService.Render("SaleOrder/RevenueReportPdf", data);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css", "print.css") },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Dự kiến doanh thu", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "Dukiendoanhthu.pdf");
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ManagementPdf([FromQuery] SaleOrderPaged val)
        {
            var data = await _saleOrderService.GetPrintManagement(val);
            var html = _viewRenderService.Render("SaleOrder/ManagementPdf", data);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css", "print.css") },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Quản lý điều trị", Right = "Page [page] of [toPage]" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            var file = _converter.Convert(pdf);
            return File(file, "application/pdf", "QuanLyDieuTri.pdf");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportRevenueReportExcel(SaleOrderRevenueReportPaged val) // xuất dự kiến doanh thu
        {
            val.Limit = 0;
            var res = await _saleOrderService.GetRevenueReport(val);
            var data = res.Items;
            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BaoCaoDuKienThu");

                worksheet.Cells["A1:E1"].Value = "BÁO CÁO DỰ KIẾN THU";
                worksheet.Cells["A1:E1"].Style.Font.Size = 14;
                worksheet.Cells["A1:E1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:E1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:E1"].Merge = true;
                worksheet.Cells["A1:E1"].Style.Font.Bold = true;
                worksheet.Cells["A2:E2"].Value = "";
                worksheet.Cells["A3"].Value = "Số phiếu";
                worksheet.Cells["B3"].Value = "Khách hàng";
                worksheet.Cells["C3"].Value = "Tiền điều trị";
                worksheet.Cells["C3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["D3"].Value = "Thanh toán";
                worksheet.Cells["D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["E3"].Value = "Còn lại";
                worksheet.Cells["E3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells["A3:E3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:E3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A3:E3"].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells["A3:E3"].Style.Font.Size = 14;

                var row = 4;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.Name;
                    worksheet.Cells[row, 2].Value = item.PartnerDisplayName;
                    worksheet.Cells[row, 3].Value = item.AmountTotal;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "#,###,###";
                    worksheet.Cells[row, 4].Value = item.TotalPaid;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "#,###,###";
                    worksheet.Cells[row, 5].Value = item.Residual;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "#,###,###";
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
        [HttpPost("[action]")]
        public async Task<IActionResult> ExportManagementExcel(SaleOrderPaged val)
        {
            val.Limit = 0;
            var salePaged = await _saleOrderService.GetPagedResultAsync(val);
            var saleIds = salePaged.Items.Select(x => x.Id).ToList();
            var allLines = await _mapper.ProjectTo<SaleOrderLineBasic>(_saleLineService.SearchQuery(x => saleIds.Contains(x.OrderId))).ToListAsync();

            var stream = new MemoryStream();
            byte[] fileContent;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("QuanLyDieuTri");

                worksheet.Cells["A1:H1"].Value = "BÁO CÁO QUẢN LÝ ĐIỀU TRỊ CHƯA HOÀN THÀNH";
                worksheet.Cells["A1:H1"].Style.Font.Size = 14;
                worksheet.Cells["A1:H1"].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:H1"].Merge = true;
                worksheet.Cells["A1:H1"].Style.Font.Bold = true;

                worksheet.Cells["A2:H2"].Value = "";
                worksheet.Cells["A3"].Value = "Ngày lập phiếu";
                worksheet.Cells["B3"].Value = "Số phiếu";
                worksheet.Cells["C3"].Value = "Khách hàng";
                worksheet.Cells["D3:E3"].Value = "Bác sĩ đại diện";
                worksheet.Cells["D3:E3"].Merge = true;
                worksheet.Cells["F3"].Value = "Tiền điều trị";
                worksheet.Cells["G3"].Value = "Thanh toán";
                worksheet.Cells["H3"].Value = "Còn lại";
                worksheet.Cells["A3:H3"].Style.Font.Bold = true;
                worksheet.Cells["A3:H3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A3:H3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#2F75B5"));
                worksheet.Cells["A3:H3"].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells["A3:H3"].Style.Font.Size = 14;

                var row = 4;
                foreach (var item in salePaged.Items)
                {
                    worksheet.Cells[row, 4, row, 5].Merge = true;

                    worksheet.Cells[row, 1].Value = item.DateOrder;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "dd/mm/yyyy";
                    worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[row, 3].Value = item.PartnerName;
                    worksheet.Cells[row, 4].Value = item.DoctorName;
                    worksheet.Cells[row, 6].Value = item.AmountTotal ?? 0;
                    worksheet.Cells[row, 7].Value = item.TotalPaid ?? 0;
                    worksheet.Cells[row, 8].Value = item.Residual ?? 0;
                    worksheet.Cells[row, 6, row, 8].Style.Numberformat.Format = "#,##0";

                    row++;

                    worksheet.Cells[row, 2].Value = "Dịch vụ";
                    worksheet.Cells[row, 2, row, 3].Merge = true;
                    worksheet.Cells[row, 5].Value = "Số lượng";
                    worksheet.Cells[row, 4].Value = "Đơn vị tính";
                    worksheet.Cells[row, 6].Value = "Thành tiền";
                    worksheet.Cells[row, 7].Value = "Thanh toán";
                    worksheet.Cells[row, 8].Value = "Còn lại";

                    worksheet.Cells[row, 2, row, 8].Style.Font.Bold = true;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 2, row, 8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#DDEBF7"));
                    worksheet.Cells[row, 2, row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    var saleLines = allLines.Where(x => x.OrderId == item.Id).ToList();
                    var rowEnd = row + saleLines.Count();
                    worksheet.Cells[row, 1, rowEnd, 1].Merge = true;
                    row++;

                    foreach (var line in saleLines)
                    {
                        worksheet.Cells[row, 2, row, 3].Merge = true;
                        worksheet.Cells[row, 2].Value = line.Name;
                        worksheet.Cells[row, 4].Value = line.ProductUOMQty;
                        worksheet.Cells[row, 5].Value = line.ProductUOMName;
                        worksheet.Cells[row, 6].Value = line.PriceSubTotal;
                        worksheet.Cells[row, 7].Value = line.AmountInvoiced ?? 0;
                        worksheet.Cells[row, 8].Value = line.AmountResidual ?? 0;
                        worksheet.Cells[row, 2, row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, 6, row, 8].Style.Numberformat.Format = "#,##0";
                        row++;
                    }

                }

                worksheet.Cells.AutoFitColumns();
                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> GetDotKhamStepByOrderLine(Guid id)
        {
            var res = await _saleOrderService.GetDotKhamStepByOrderLine(id);
            return Ok(res);
        }

        [HttpPost("{id}/[action]")]
        [CheckAccess(Actions = "Basic.DotKham.Create")]
        public async Task<IActionResult> CreateDotKham(Guid id, DotKhamSaveVm val)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var res = await _dotKhamService.CreateDotKham(id, val);
            var viewdotkham = _mapper.Map<DotKhamVm>(res);
            _unitOfWork.Commit();

            return Ok(viewdotkham);
        }


        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.DotKham.Read")]
        public async Task<IActionResult> GetDotKhamListIds(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var dotKhamIds = await _dotKhamService.SearchQuery(x => x.SaleOrderId == id).OrderByDescending(x => x.DateCreated).Select(x => x.Id).ToListAsync();

            return Ok(dotKhamIds);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\sale_order.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery().ToListAsync();// các irmodel cần thiết
            var entities = await _saleOrderService.SearchQuery(x => x.DateOrder.Date <= dateToData.Date).Include(x => x.OrderLines).ThenInclude(x => x.SaleOrderLineToothRels).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<SaleOrderXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<SaleOrderXmlSampleDataRecord>(entity);
                item.Id = $@"sample.sale_order_{entities.IndexOf(entity) + 1}";
                var irmodelDataPartner = listIrModelData.FirstOrDefault(x => x.ResId == entity.PartnerId.ToString());
                item.PartnerId = irmodelDataPartner?.Module + "." + irmodelDataPartner?.Name;
                item.DateRound = (int)(dateToData.Date - entity.DateOrder.Date).TotalDays;
                item.TimeHour = entity.DateOrder.Hour;
                item.TimeMinute = entity.DateOrder.Minute;
                //add lines
                foreach (var lineEntity in entity.OrderLines)
                {
                    var itemLine = _mapper.Map<SaleOrderLineXmlSampleDataRecord>(lineEntity);
                    itemLine.Id = $@"sample.sale_order_{entities.IndexOf(entity) + 1}_sale_order_line_{entity.OrderLines.ToList().IndexOf(lineEntity) + 1}";
                    itemLine.DateRound = (int)(dateToData.Date - lineEntity.DateCreated.Value.Date).TotalDays;
                    var irmodelDataProduct = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductId.ToString());
                    itemLine.ProductId = irmodelDataProduct?.Module + "." + irmodelDataProduct?.Name;
                    var irmodelDataToothCategory = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ToothCategoryId.ToString());
                    itemLine.ToothCategoryId = irmodelDataToothCategory?.Module + "." + irmodelDataToothCategory?.Name;

                    foreach (var lineTooth in lineEntity.SaleOrderLineToothRels)
                    {
                        var itemLineTooth = new SaleOrderLineToothRelXmlSampleDataRecord();
                        var itemLineToothIrModelData = listIrModelData.FirstOrDefault(x => x.ResId == lineTooth.ToothId.ToString());
                        itemLineTooth.ToothId = itemLineToothIrModelData?.Module + "." + itemLineToothIrModelData?.Name;
                        itemLine.SaleOrderLineToothRels.Add(itemLineTooth);
                    }
                    item.OrderLines.Add(itemLine);

                    // add IRModelData
                    irModelCreate.Add(new IRModelData()
                    {
                        Module = "sample",
                        Model = "sale.order.line",
                        ResId = lineEntity.Id.ToString(),
                        Name = $"sale_order_{entities.IndexOf(entity) + 1}_sale_order_line_{entity.OrderLines.ToList().IndexOf(lineEntity) + 1}"
                    });
                }
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "sale.order",
                    ResId = entity.Id.ToString(),
                    Name = $"sale_order_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetListAttachment(Guid id)
        {
            var res = await _saleOrderService.GetListAttachment(id);
            return Ok(_mapper.Map<IEnumerable<IrAttachmentBasic>>(res));
        }

        [HttpGet("{id}/[action]")]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public async Task<IActionResult> GetPromotions(Guid id)
        {
            var res = await _saleOrderPromotionService.SearchQuery(x => x.SaleOrderId == id).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<SaleOrderPromotionBasic>>(res));
        }
    }
}