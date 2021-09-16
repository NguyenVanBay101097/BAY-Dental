using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class PrintTemplateConfigService : BaseService<PrintTemplateConfig>, IPrintTemplateConfigService
    {
        private readonly IMapper _mapper;
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IPrintPaperSizeService _printPaperSizeService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IIRModelDataService _modelData;

        public PrintTemplateConfigService(IAsyncRepository<PrintTemplateConfig> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IPrintTemplateService printTemplateService, IPrintPaperSizeService printPaperSizeService, IWebHostEnvironment webHostEnvironment, IIRModelDataService modelData)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _printTemplateService = printTemplateService;
            _printPaperSizeService = printPaperSizeService;
            _webHostEnvironment = webHostEnvironment;
            _modelData = modelData;
        }

        public async Task<PrintTemplateConfig> GetPrintTemplateConfig(string type)
        {         
            var printConfig = await SearchQuery(x => x.Type == type && x.IsDefault).Include(x => x.PrintPaperSize).FirstOrDefaultAsync();          

            if (printConfig == null)
            {
                var printTmp = await _printTemplateService.SearchQuery(x => x.Type == type).FirstOrDefaultAsync();

                if (printTmp == null)
                    throw new Exception("Không tìm thấy mẫu in có sẵn");

                var paperSize = await _modelData.GetRef<PrintPaperSize>("base.paperformat_a4");

                printConfig = new PrintTemplateConfig();
                printConfig.Content = printTmp.Content;
                printConfig.Type = printTmp.Type;
                printConfig.PrintPaperSizeId = paperSize.Id;
                printConfig.PrintPaperSize = paperSize;
            }

            return printConfig;
        }

        public async Task<PrintTemplateConfig> ChangePaperSize(string type , Guid paperSizeId)
        {
            var printConfig = await SearchQuery(x => x.Type == type && x.PrintPaperSizeId == paperSizeId).Include(x => x.PrintPaperSize).FirstOrDefaultAsync();
            if (printConfig == null)
            {
                var printTmp = await _printTemplateService.SearchQuery(x => x.Type == type).FirstOrDefaultAsync();

                if (printTmp == null)
                    throw new Exception("Không tìm thấy mẫu in có sẵn");

                var paperSize = await _modelData.GetRef<PrintPaperSize>("base.paperformat_a4");

                printConfig = new PrintTemplateConfig();
                printConfig.Content = printTmp.Content;
                printConfig.Type = printTmp.Type;
                printConfig.PrintPaperSizeId = paperSize.Id;
                printConfig.PrintPaperSize = paperSize;
            }

            return printConfig;
        }
  

        public async Task<object> GetSampleData(string type)
        {
            var comObj = GetService<ICompanyService>();
            var company = await comObj.SearchQuery(x => x.Id == CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
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
                        var res_toathuoc = JsonConvert.DeserializeObject<ToaThuoc>(item.Data.ToString());
                        res_toathuoc.Company = company;
                        res_toathuoc.ReExaminationDate = DateTime.Now.AddMonths(3);
                        obj = _mapper.Map<ToaThuocPrintViewModel>(res_toathuoc);
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
                        obj = JsonConvert.DeserializeObject<AccountPaymentPrintVM>(item.Data.ToString());
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
                    default:
                        break;
                }
            }



            return obj;
        }

        public string ConnectLayoutForContent(string layout , string content)
        {          
            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.SelectSingleNode("//div[@class='container']").InnerHtml += content;
            var newHtml = doc.DocumentNode.OuterHtml;
            return newHtml;
        }

        public async Task<string> PrintTest(string content , string type , Guid paperSizeId)
        {
            object data = await GetSampleData(type);
            var printPaperSize = await _printPaperSizeService.SearchQuery(x => x.Id == paperSizeId).FirstOrDefaultAsync();                
            var layout = File.ReadAllText("PrintTemplate/Shared/Layout.html");

            var renderContent = await RenderTemplate(data, content);
            var renderLayout = await RenderTemplate(printPaperSize, layout);

            var result = ConnectLayoutForContent(renderLayout, renderContent);

            return result;
        }

        public async Task<string> Print(object data , string type)// in hóa đơn của 1 type cụ thể kèm data
        {
            
            var printConfig = await GetPrintTemplateConfig(type);
            var renderContent = await RenderTemplate(data, printConfig.Content);

            var printPaperSize = await _printPaperSizeService.SearchQuery(x => x.Id == printConfig.PrintPaperSizeId).FirstOrDefaultAsync();
            var layoutHtml = File.ReadAllText("PrintTemplate/Shared/Layout.html");
            var renderLayout = await RenderTemplate(printPaperSize, layoutHtml);        

            var result = ConnectLayoutForContent(renderLayout,renderContent);

            return result;
        }

        public async Task<string> RenderTemplate(object data, string content)
        {        
            var template = Template.Parse(content);
            var result = await template.RenderAsync(data);
            return result;
        }

        public override ISpecification<PrintTemplateConfig> RuleDomainGet(IRRule rule)
        {
            switch (rule.Code)
            {
                case "base.print_template_config_comp_rule":
                    return new InitialSpecification<PrintTemplateConfig>(x => x.CompanyId == CompanyId);
                default:
                    return null;
            }
        }



    }
}
