using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;
using Scriban;
using Scriban.Runtime;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using ApplicationCore.Models.PrintTemplate;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintTemplateConfigsController : BaseApiController
    {
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintPaperSizeService _printPaperSizeService;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IMapper _mapper;
        private readonly ICompanyService _companyService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PrintTemplateConfigsController(IPrintTemplateConfigService printTemplateConfigService, IPrintPaperSizeService printPaperSizeService, IPrintTemplateService printTemplateService, IMapper mapper,
            ICompanyService companyService, IWebHostEnvironment webHostEnvironment
            )
        {
            _printTemplateConfigService = printTemplateConfigService;
            _printPaperSizeService = printPaperSizeService;
            _printTemplateService = printTemplateService;
            _mapper = mapper;
            _companyService = companyService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDisplay(string type)
        {
            if (!ModelState.IsValid || type == null)
                return BadRequest();
            var res = await _printTemplateConfigService.GetPrintTemplateConfig(type);
            var display = _mapper.Map<PrintTemplateConfigDisplay>(res);
            return Ok(display);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePaperSize(PrintTemplateConfigChangePaperSize val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            var res = await _printTemplateConfigService.ChangePaperSize(val.Type, val.PrintPaperSizeId);
            var display = _mapper.Map<PrintTemplateConfigDisplay>(res);
            return Ok(display);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> CreateOrUpdate(PrintTemplateConfigSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var printConfig = await _printTemplateConfigService.SearchQuery(x => x.Type == val.Type && x.CompanyId == val.CompanyId && x.PrintPaperSizeId == val.PrintPaperSizeId)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            if (printConfig != null)
            {
                printConfig.IsDefault = true;
                await _printTemplateConfigService.UpdateAsync(printConfig);

                var template = printConfig.PrintTemplate;
                template.Content = val.Content;
                await _printTemplateService.UpdateAsync(template);
            }
            else
            {
                var model = _printTemplateService.GetModelTemplate(val.Type);

                var template = new PrintTemplate { Content = val.Content, Model = model };
                await _printTemplateService.CreateAsync(template);

                var res = _mapper.Map<PrintTemplateConfig>(val);
                res.PrintTemplateId = template.Id;
                res.IsDefault = true;

                await _printTemplateConfigService.CreateAsync(res);
                printConfig = res;
            }

            var otherConfigs = await _printTemplateConfigService.SearchQuery(x => x.Type == val.Type && x.CompanyId == val.CompanyId && x.Id != printConfig.Id)
               .ToListAsync();

            foreach (var config in otherConfigs)
                config.IsDefault = false;

            await _printTemplateConfigService.UpdateAsync(otherConfigs);

            return NoContent();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Generate(GenerateReq val)
        {
            object data = await GetSampleData(val.Type);
            var result = await _printTemplateConfigService.RenderTemplate(data, val.Content);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PrintTest(PrintTestReq val)
        {
            if (string.IsNullOrEmpty(val.Content))
                throw new Exception("Mẫu in rỗng , cấu hình mãu in để in thử");
            object data = await GetSampleData(val.Type);
            var result = await _printTemplateConfigService.PrintTest(val.Content, val.PrintPaperSizeId, data);
            return Ok(result);

        }

        private async Task<object> GetSampleData(string type)
        {
            var company = await _companyService.SearchQuery(x => x.Id == CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\print_template_data.json");
            object obj = new object();

            using (var reader = new StreamReader(filePath))
            {
                var fileContent = reader.ReadToEnd();
                var sample_data = JsonConvert.DeserializeObject<List<SampleDataPrintTemplate>>(fileContent);
                var item = sample_data.Where(s => s.Type == type).FirstOrDefault();

                switch (type)
                {
                    case "tmp_sale_order":
                        obj = JsonConvert.DeserializeObject<SaleOrderPrintVM>(item.Data.ToString());
                        break;
                    case "tmp_toathuoc":
                        obj = JsonConvert.DeserializeObject<ToaThuoc>(item.Data.ToString());
                        break;
                    case "tmp_labo_order":
                        var res_labo = JsonConvert.DeserializeObject<LaboOrderPrintVM>(item.Data.ToString());
                        res_labo.Company = _mapper.Map<CompanyPrintVM>(company);
                        obj = res_labo;
                        break;
                    case "tmp_purchase_order":
                        var res_purchase_order = JsonConvert.DeserializeObject<PurchaseOrder>(item.Data.ToString());
                        res_purchase_order.Company = company;
                        obj = _mapper.Map<PurchaseOrderPrintVm>(res_purchase_order);
                        break;
                    case "tmp_purchase_refund":
                        var res_purchase_refund = JsonConvert.DeserializeObject<PurchaseOrder>(item.Data.ToString());
                        res_purchase_refund.Company = company;
                        obj = _mapper.Map<PurchaseOrderPrintVm>(res_purchase_refund);

                        break;
                    case "tmp_medicine_order":
                        var res_medicine_order = JsonConvert.DeserializeObject<MedicineOrder>(item.Data.ToString());
                        res_medicine_order.Company = company;
                        obj = _mapper.Map<MedicineOrderPrint>(res_medicine_order);
                        break;
                    case "tmp_phieu_thu":
                        var res_phieu_thu = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                        res_phieu_thu.Company = company;
                        obj = _mapper.Map<PhieuThuChiPrintVM>(res_phieu_thu);
                        break;
                    case "tmp_phieu_chi":
                        var res_medicine_chi = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                        res_medicine_chi.Company = company;
                        obj = _mapper.Map<PhieuThuChiPrintVM>(res_medicine_chi);
                        break;
                    case "tmp_customer_debt":
                        var res_customer_debt = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                        res_customer_debt.Company = company;
                        obj = _mapper.Map<PrintVM>(res_customer_debt);
                        break;
                    case "tmp_agent_commission":
                        var res_agent_commission = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                        res_agent_commission.Company = company;
                        obj = _mapper.Map<PrintVM>(res_agent_commission);
                        break;
                    case "tmp_stock_picking_incoming":
                        obj = JsonConvert.DeserializeObject<StockPickingPrintVm>(item.Data.ToString());
                        break;
                    case "tmp_stock_picking_outgoing":
                        obj = JsonConvert.DeserializeObject<StockPickingPrintVm>(item.Data.ToString());
                        break;
                    case "tmp_stock_inventory":
                        obj = JsonConvert.DeserializeObject<StockInventoryPrint>(item.Data.ToString());
                        break;
                    case "tmp_salary_employee":
                        obj = JsonConvert.DeserializeObject<SalaryPaymentPrint>(item.Data.ToString());
                        break;
                    case "tmp_salary_advance":
                        obj = JsonConvert.DeserializeObject<SalaryPaymentPrint>(item.Data.ToString());
                        break;
                    case "tmp_salary":
                        obj = JsonConvert.DeserializeObject<HrPayslipRunPrintVm>(item.Data.ToString());
                        break;
                    case "tmp_advisory":
                        obj = JsonConvert.DeserializeObject<AdvisoryPrintVM>(item.Data.ToString());
                        break;
                    case "tmp_account_payment":
                        obj = JsonConvert.DeserializeObject<SaleOrderPaymentPrintVM>(item.Data.ToString());
                        break;
                    case "tmp_supplier_payment":
                        obj = JsonConvert.DeserializeObject<AccountPaymentPrintTemplate>(item.Data.ToString());
                        break;
                    case "tmp_partner_advance":
                        obj = JsonConvert.DeserializeObject<PartnerAdvancePrint>(item.Data.ToString());
                        (obj as PartnerAdvancePrint).Company = _mapper.Map<CompanyPrintVM>(company);
                        break;
                    case "tmp_partner_refund":
                        obj = JsonConvert.DeserializeObject<PartnerAdvancePrint>(item.Data.ToString());
                        (obj as PartnerAdvancePrint).Company = _mapper.Map<CompanyPrintVM>(company);
                        break;
                    case "tmp_quotation":
                        obj = JsonConvert.DeserializeObject<QuotationPrintVM>(item.Data.ToString());
                        (obj as QuotationPrintVM).Company = _mapper.Map<CompanyPrintVM>(company);
                        break;
                    case "tmp_supplier_payment_inbound":
                        obj = JsonConvert.DeserializeObject<AccountPaymentPrintTemplate>(item.Data.ToString());
                        break;
                    default:
                        break;
                }
            }



            return obj;
        }
    }
}
