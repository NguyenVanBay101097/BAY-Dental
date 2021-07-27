using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderLinesController : BaseApiController
    {
        private readonly ISaleOrderLineService _saleLineService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IExportExcelService _exportExcelService;
        public SaleOrderLinesController(ISaleOrderLineService saleLineService, IMapper mapper,IExportExcelService exportExcelService,
        IUnitOfWorkAsync unitOfWork)
        {
            _saleLineService = saleLineService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _exportExcelService = exportExcelService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaleOrderLineSave val)
        {
            await _unitOfWork.BeginTransactionAsync();
            var line = await _saleLineService.CreateOrderLine(val);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderLineDisplay>(line);
            return Ok(basic);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, SaleOrderLineSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _saleLineService.UpdateOrderLine(id, val);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleLineService.RemoveOrderLine(id);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("OnChangeProduct")]
        public async Task<IActionResult> OnChangeProduct(SaleOrderLineOnChangeProduct val)
        {
            var res = await _saleLineService.OnChangeProduct(val);
            return Ok(res);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetDisplayBySaleOrder(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var res = await _saleLineService.GetDisplayBySaleOrder(id);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderLinesPaged val)
        {
            var result = await _saleLineService.GetPagedResultAsync(val);

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CancelSaleOrderLine(IEnumerable<Guid> Ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleLineService.CancelSaleOrderLine(Ids);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetLaboOrders(Guid id)
        {
            var res = await _saleLineService.GetLaboOrderBasics(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyDiscountOnOrderLine(ApplyDiscountViewModel val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleLineService.ApplyDiscountOnOrderLine(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotionUsageCode(ApplyPromotionUsageCode val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var res = await _saleLineService.ApplyPromotionUsageCodeOnOrderLine(val);
            _unitOfWork.Commit();
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ApplyPromotion(ApplyPromotionRequest val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            await _saleLineService.ApplyPromotionOnOrderLine(val);
            _unitOfWork.Commit();
            return NoContent();
        }

        [HttpPatch("{id}/[action]")]
        public async Task<IActionResult> PatchIsActive(Guid id, SaleOrderLineIsActivePatch result)
        {
            var entity = await _saleLineService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var patch = new JsonPatchDocument<SaleOrderLineIsActivePatch>();
            patch.Replace(x => x.IsActive, result.IsActive);
            var entityMap = _mapper.Map<SaleOrderLineIsActivePatch>(entity);
            patch.ApplyTo(entityMap);

            entity = _mapper.Map(entityMap, entity);
            await _saleLineService.UpdateAsync(entity);

            return NoContent();
        }

        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetTeeth(Guid id)
        {
            var res = await _saleLineService.GetTeeth(id);
            return Ok(res);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Labo.LaboOrder.Read")]
        public async Task<IActionResult> GetListLineIsLabo([FromQuery] SaleOrderLinesLaboPaged val)
        {
            var query = _saleLineService.SearchQuery(x => x.Product.IsLabo);

            if (!string.IsNullOrEmpty(val.Search))
            {
                query = query.Where(x => x.OrderPartner.Name.Contains(val.Search) ||
                x.OrderPartner.NameNoSign.Contains(val.Search) || x.OrderPartner.Ref.Contains(val.Search) || x.Order.Name.Contains(val.Search));
            }

            if (val.HasAnyLabo.HasValue)
            {
                if (!val.HasAnyLabo.Value)
                    query = query.Where(x => !x.Labos.Any());
                else
                    query = query.Where(x => x.Labos.Any());
            }

            if (!string.IsNullOrEmpty(val.LaboState))
            {
                if (val.LaboState == "draft")
                    query = query.Where(x => x.Labos.Any(s => s.State == "draft"));
                else if (val.LaboState == "confirmed")
                    query = query.Where(x => x.Labos.Any(s => s.State == "confirmed"));
            }

            var totalItems = await query.CountAsync();

            query = query.Include(x => x.OrderPartner)
                .Include(x => x.Order)
                .Include(x => x.Employee)
                .Include(x => x.Labos)
                .Include(x => x.SaleOrderLineToothRels).ThenInclude(x => x.Tooth);

            query = query.OrderByDescending(x => x.DateCreated);

            var items = await query.Skip(val.Offset).Take(val.Limit).ToListAsync();

            var paged = new PagedResult2<SaleOrderLineBasic>(totalItems, val.Offset, val.Limit)
            {
                Items = _mapper.Map<IEnumerable<SaleOrderLineBasic>>(items)
            };

            return Ok(paged);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSmsCareAfterOrderManual([FromQuery] SmsCareAfterOrderPaged val)
        {
            var result = await _saleLineService.GetPagedSmsCareAfterOrderAsync(val);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductSmsCareAfterOrder(DateTime? dateFrom, DateTime? dateTo, Guid? companyId)
        {
            var result = await _saleLineService.GetProductSmsCareAfterOrder(dateFrom, dateTo, companyId);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistory([FromQuery]SaleOrderLineHistoryReq val)
        {
            var res = await _saleLineService.GetHistory(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSaleReportExportExcel([FromQuery] SaleOrderLinesPaged val)
        {
            val.Limit = int.MaxValue;
            var res = await _saleLineService.GetPagedResultAsync(val);
            var data = _mapper.Map<IEnumerable<ServiceSaleReportExcel>>(res.Items);
            byte[] fileContent;
            var packageByte = await _exportExcelService.createExcel(data, "báo cáo dịch vụ");
            using (MemoryStream memStream = new MemoryStream(packageByte as byte[]))
            {
                //ExcelPackage packageResult = new ExcelPackage(memStream);
                //packageResult.Load(memStream);
                ////await _exportExcelService.AddToHeader(packageResult.GetAsByteArray());
                //packageResult.Save();

                //fileContent = memStream.ToArray();
                //string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //return new FileContentResult(fileContent, mimeType);
                ExcelPackage package = new ExcelPackage(memStream);
                package.Load(memStream);

                var worksheet = package.Workbook.Worksheets[0];
                worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;

                //insert title
                worksheet.InsertRow(1,3);
                worksheet.Cells[1,1,1,worksheet.Dimension.Columns].Merge = true;
                worksheet.Cells[1, 1].Value = "BÁO CÁO DỊCH VỤ ĐANG ĐIỀU TRỊ";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#6ca4cc"));
                worksheet.Cells[2,1,2,worksheet.Dimension.Columns].Merge = true;
                worksheet.Cells[2, 1].Value = (val.DateOrderFrom.HasValue ? "Từ ngày " + val.DateOrderFrom.Value.ToString("dd/MM/yyyy") : "") +
                                               (val.DateOrderTo.HasValue ? " đến ngày " + val.DateOrderTo.Value.ToString("dd/MM/yyyy") : "");


                worksheet.Cells.AutoFitColumns();
                


                await _exportExcelService.AddToHeader(package.GetAsByteArray());
            }

            return Ok();
          
        }

        [HttpPut("{id}/[action]")]
        public async Task<IActionResult> UpdateState(Guid id, string state)
        {
            await _saleLineService.UpdateState(id,state);
            return Ok();
        }
    }
}