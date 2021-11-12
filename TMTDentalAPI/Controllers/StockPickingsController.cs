using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockPickingsController : BaseApiController
    {
        private readonly IStockPickingService _stockPickingService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IStockMoveService _stockMoveService;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public StockPickingsController(IStockPickingService stockPickingService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork, IIRModelAccessService modelAccessService, IStockMoveService stockMoveService, IViewRenderService viewRenderService,
             IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService
            , IPrintTemplateConfigService printTemplateConfigService)
        {
            _stockPickingService = stockPickingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _modelAccessService = modelAccessService;
            _stockMoveService = stockMoveService;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        [CheckAccess(Actions = "Stock.Picking.Read")]
        public async Task<IActionResult> Get([FromQuery] StockPickingPaged val)
        {
            var res = await _stockPickingService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "Stock.Picking.Read")]
        public async Task<IActionResult> Get(Guid id)
        {
            var picking = await _stockPickingService.GetPickingForDisplay(id);
            if (picking == null)
                return NotFound();
            var res = _mapper.Map<StockPickingDisplay>(picking);
            res.MoveLines = res.MoveLines.OrderBy(x => x.Sequence);
            return Ok(res);
        }

        [HttpPost]
        [CheckAccess(Actions = "Stock.Picking.Create")]
        public async Task<IActionResult> Create(StockPickingDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var picking = _mapper.Map<StockPicking>(val);
            SaveMoveLines(val, picking);
            _stockMoveService._Compute(picking.MoveLines);

            await _unitOfWork.BeginTransactionAsync();
            await _stockPickingService.CreateAsync(picking);
            _unitOfWork.Commit();

            val.Id = picking.Id;
            return Ok(val);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "Stock.Picking.Update")]
        public async Task<IActionResult> Update(Guid id, StockPickingDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var picking = await _stockPickingService.GetPickingForDisplay(id);
            if (picking == null)
                return NotFound();

            picking = _mapper.Map(val, picking);
            SaveMoveLines(val, picking);

            _stockMoveService._Compute(picking.MoveLines);

            await _stockPickingService.UpdateAsync(picking);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "Stock.Picking.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _stockPickingService.Unlink(new List<Guid>() { id });
            _unitOfWork.Commit();
            return NoContent();
        }

        private void SaveMoveLines(StockPickingDisplay val, StockPicking picking)
        {
            if (picking.State != "draft")
                return;

            //remove line
            var lineToRemoves = new List<StockMove>();
            foreach (var existLine in picking.MoveLines)
            {
                if (!val.MoveLines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                picking.MoveLines.Remove(line);
            }

            int sequence = 1;
            foreach (var line in val.MoveLines)
                line.Sequence = sequence++;

            foreach (var line in val.MoveLines)
            {
                if (line.Id == Guid.Empty)
                {
                    var ml = _mapper.Map<StockMove>(line);
                    ml.Date = picking.Date ?? DateTime.Now;
                    ml.CompanyId = picking.CompanyId;
                    ml.LocationId = picking.LocationId;
                    ml.LocationDestId = picking.LocationDestId;
                    picking.MoveLines.Add(ml);
                }
                else
                {
                    _mapper.Map(line, picking.MoveLines.SingleOrDefault(c => c.Id == line.Id));
                }
            }
        }

        [HttpPost("DefaultGet")]
        public async Task<IActionResult> DefaultGet(StockPickingDefaultGet val)
        {
            var res = await _stockPickingService.DefaultGet(val);
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGetOutgoing()
        {
            var res = await _stockPickingService.DefaultGetOutgoing();
            return Ok(res);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DefaultGetIncoming()
        {
            var res = await _stockPickingService.DefaultGetIncoming();
            return Ok(res);
        }

        [HttpPost("ActionDone")]
        [CheckAccess(Actions = "Stock.Picking.Update")]
        public async Task<IActionResult> ActionDone(IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _stockPickingService.ActionDone(ids);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpPost("GetPaged")]
        [CheckAccess(Actions = "Stock.Picking.Read")]
        public async Task<IActionResult> GetPaged(StockPickingPaged val)
        {
            var result = await _stockPickingService.GetPagedResultAsync(val);

            var paged = new PagedResult2<StockPickingBasic>(result.TotalItems, result.Offset, result.Limit)
            {
                //Có thể dùng thư viện automapper
                Items = _mapper.Map<IEnumerable<StockPickingBasic>>(result.Items),
            };

            return Ok(paged);
        }

        [HttpPost("OnChangePickingType")]
        public async Task<IActionResult> OnChangePickingType(StockPickingOnChangePickingType val)
        {
            var res = await _stockPickingService.OnChangePickingType(val);
            return Ok(res);
        }

        [HttpPost("{id}/Print")]
        [CheckAccess(Actions = "Stock.Picking.Read")]
        public async Task<IActionResult> Print(Guid id)
        {

            var res = await _stockPickingService.SearchQuery(x => x.Id == id).Include(x => x.PickingType).FirstOrDefaultAsync();
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == (res.PickingType.Code == "outgoing" ? "tmp_stock_picking_outgoing" : "tmp_stock_picking_incoming") && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>(res.PickingType.Code == "outgoing" ? "base.print_template_stock_picking_outgoing" : "base.print_template_stock_picking_incoming");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            //chonj partner cuar m=employee vafo 
            var empObj = (IEmployeeService)HttpContext.RequestServices.GetService(typeof(IEmployeeService));
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\stock_picking.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "stock" || x.Module == "account" || x.Module == "product")).ToListAsync();// các irmodel cần thiết
            var entities = await _stockPickingService.SearchQuery(x => x.MoveLines.All(z => z.PurchaseLineId == null) && x.Date.Value.Date <= dateToData.Date).Include(x => x.MoveLines).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<StockPickingXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var empPartner = await empObj.SearchQuery(x => x.PartnerId == entity.PartnerId).FirstOrDefaultAsync();

                var item = _mapper.Map<StockPickingXmlSampleDataRecord>(entity);

                item.Id = $@"sample.stock_picking_{entities.IndexOf(entity) + 1}";
                item.DateRound = (int)(dateToData - entity.Date.Value).TotalDays;
                var irmodelDataPartner = listIrModelData.FirstOrDefault(x => x.ResId == empPartner.Id.ToString());
                item.PartnerId = irmodelDataPartner == null ? "" : irmodelDataPartner?.Module + "." + irmodelDataPartner?.Name;
                var irmodelDataPickingType = listIrModelData.FirstOrDefault(x => x.ResId == entity.PickingTypeId.ToString());
                item.PickingTypeId = irmodelDataPickingType == null ? "" : irmodelDataPickingType.Module + "." + irmodelDataPickingType?.Name;
                var irmodelDataLocation = listIrModelData.FirstOrDefault(x => x.ResId == entity.LocationId.ToString());
                item.LocationId = irmodelDataLocation == null ? "" : irmodelDataLocation.Module + "." + irmodelDataLocation?.Name;
                var irmodelDataLocationDes = listIrModelData.FirstOrDefault(x => x.ResId == entity.LocationDestId.ToString());
                item.LocationDestId = irmodelDataLocationDes == null ? "" : irmodelDataLocationDes.Module + "." + irmodelDataLocationDes?.Name;
                //add lines
                foreach (var lineEntity in entity.MoveLines)
                {
                    var irmodelDataLocationLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.LocationId.ToString());
                    var irmodelDataLocationDesLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.LocationDestId.ToString());
                    //var irmodelDataPartnerLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.PartnerId.ToString());
                    var irmodelDataPickingTypeLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.PickingTypeId.ToString());
                    var irmodelDataProductLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductId.ToString());
                    var irmodelDataProductUomLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.ProductUOMId.ToString());

                    var itemLine = new StockMoveXmlSampleDataRecord()
                    {
                        DateRound = (int)(dateToData - lineEntity.Date).TotalDays,
                        LocationId = irmodelDataLocationLine == null ? "" : irmodelDataLocationLine.Module + "." + irmodelDataLocationLine?.Name,
                        LocationDestId = irmodelDataLocationDesLine == null ? "" : irmodelDataLocationDesLine.Module + "." + irmodelDataLocationDesLine?.Name,
                        Name = lineEntity.Name,
                        PartnerId = irmodelDataPartner == null ? "" : irmodelDataPartner.Module + "." + irmodelDataPartner?.Name,
                        PickingTypeId = irmodelDataPickingTypeLine == null ? "" : irmodelDataPickingTypeLine.Module + "." + irmodelDataPickingTypeLine?.Name,
                        PriceUnit = lineEntity.PriceUnit,
                        ProductId = irmodelDataProductLine == null ? "" : irmodelDataProductLine.Module + "." + irmodelDataProductLine?.Name,
                        ProductQty = lineEntity.ProductQty,
                        ProductUOMId = irmodelDataProductUomLine == null ? "" : irmodelDataProductUomLine.Module + "." + irmodelDataProductUomLine?.Name,
                        ProductUOMQty = lineEntity.ProductUOMQty
                    };
                    item.MoveLines.Add(itemLine);
                }
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "stock.picking",
                    ResId = entity.Id.ToString(),
                    Name = $"stock_picking_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}