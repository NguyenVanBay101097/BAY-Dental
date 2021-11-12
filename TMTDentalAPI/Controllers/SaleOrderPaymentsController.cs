using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ApplicationCore.Entities;
using ApplicationCore.Utilities;
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
    public class SaleOrderPaymentsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ISaleOrderPaymentService _saleOrderPaymentService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IViewRenderService _viewRenderService;
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IIRModelDataService _modelDataService;

        public SaleOrderPaymentsController(IMapper mapper, ISaleOrderPaymentService saleOrderPaymentService, IUnitOfWorkAsync unitOfWork, IViewRenderService viewRenderService,
            IPrintTemplateConfigService printTemplateConfigService,
            IPrintTemplateService printTemplateService,
            IIRModelDataService modelDataService
            )
        {
            _mapper = mapper;
            _saleOrderPaymentService = saleOrderPaymentService;
            _unitOfWork = unitOfWork;
            _viewRenderService = viewRenderService;
            _printTemplateConfigService = printTemplateConfigService;
            _printTemplateService = printTemplateService;
            _modelDataService = modelDataService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SaleOrderPaymentPaged val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHistoryPartnerAdvance([FromQuery] HistoryPartnerAdvanceFilter val)
        {
            var result = await _saleOrderPaymentService.GetPagedResultHistoryAdvanceAsync(val);
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
        [CheckAccess(Actions = "Basic.SaleOrderPayment.Full")]
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
        [CheckAccess(Actions = "Basic.SaleOrderPayment.Full")]
        public async Task<IActionResult> ActionCancel(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _saleOrderPaymentService.ActionCancel(ids);
            _unitOfWork.Commit();
            return NoContent();
        }



        [HttpGet("{id}/[action]")]
        public async Task<IActionResult> GetPrint(Guid id)
        {
            //tim trong bảng config xem có dòng nào để lấy ra template
            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == "tmp_account_payment" && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            PrintTemplate template = printConfig != null ? printConfig.PrintTemplate : null;
            PrintPaperSize paperSize = printConfig != null ? printConfig.PrintPaperSize : null;
            if (template == null)
            {
                //tìm template mặc định sử dụng chung cho tất cả chi nhánh, sử dụng bảng IRModelData hoặc bảng IRConfigParameter
                template = await _modelDataService.GetRef<PrintTemplate>("base.print_template_account_payment");
                if (template == null)
                    throw new Exception("Không tìm thấy mẫu in mặc định");
            }

            var result = await _printTemplateService.GeneratePrintHtml(template, new List<Guid>() { id }, paperSize);

            return Ok(new PrintData() { html = result });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GenerateXML()
        {
            var irModelObj = (IIRModelDataService)HttpContext.RequestServices.GetService(typeof(IIRModelDataService));
            var _hostingEnvironment = (IWebHostEnvironment)HttpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
            var xmlService = (IXmlService)HttpContext.RequestServices.GetService(typeof(IXmlService));
            string path = Path.Combine(_hostingEnvironment.ContentRootPath, @"SampleData\ImportXML\sale_order_payment.xml");

            var irModelCreate = new List<IRModelData>();
            var dateToData = new DateTime(2021, 08, 25);
            var listIrModelData = await irModelObj.SearchQuery(x => (x.Module == "sample" || x.Module == "account")).ToListAsync();// các irmodel cần thiết
            var entities = await _saleOrderPaymentService.SearchQuery(x => x.Date.Date <= dateToData.Date).Include(x => x.JournalLines).Include(x => x.Order.OrderLines).Include(x => x.Lines).ToListAsync();//lấy dữ liệu mẫu: bỏ dữ liệu mặc định
            var data = new List<SaleOrderPaymentXmlSampleDataRecord>();
            foreach (var entity in entities)
            {
                var item = _mapper.Map<SaleOrderPaymentXmlSampleDataRecord>(entity);
                item.Id = $@"sample.sale_order_payment_{entities.IndexOf(entity) + 1}";
                var irmodelDataOrder = listIrModelData.FirstOrDefault(x => x.ResId == entity.OrderId.ToString());
                item.OrderId = irmodelDataOrder?.Module + "." + irmodelDataOrder?.Name;
                item.DateRound = (int)(dateToData - entity.Date).TotalDays;
                //add lines
                foreach (var lineEntity in entity.Lines)
                {
                    var irmodelDataOrderLine = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.SaleOrderLineId.ToString());
                    var itemLine = new SaleOrderPaymentHistoryLineXmlSampleDataRecord()
                    {
                        Amount = lineEntity.Amount,
                        SaleOrderLineId = irmodelDataOrderLine?.Module + "." + irmodelDataOrderLine?.Name
                    };
                    item.Lines.Add(itemLine);
                }
                //add lines
                foreach (var lineEntity in entity.JournalLines)
                {
                    var irmodelDataJournal = listIrModelData.FirstOrDefault(x => x.ResId == lineEntity.JournalId.ToString());
                    var itemLine = new SaleOrderPaymentJournalLineXmlSampleDataRecord()
                    {
                        Amount = lineEntity.Amount,
                        JournalId = irmodelDataJournal?.Module + "." + irmodelDataJournal?.Name
                    };
                    item.JournalLines.Add(itemLine);
                }
                data.Add(item);
                // add IRModelData
                irModelCreate.Add(new IRModelData()
                {
                    Module = "sample",
                    Model = "sale.order.payment",
                    ResId = entity.Id.ToString(),
                    Name = $"sale_order_payment_{entities.IndexOf(entity) + 1}"
                });
            }
            //writeFile
            xmlService.WriteXMLFile(path, data);
            await irModelObj.CreateAsync(irModelCreate);
            return Ok();
        }
    }
}
