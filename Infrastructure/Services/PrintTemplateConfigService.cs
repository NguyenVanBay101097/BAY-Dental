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
using System.Globalization;
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
            var printConfig = await SearchQuery(x => x.Type == type && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            if (printConfig == null)
            {
                var templateObj = GetService<IPrintTemplateService>();
                var printTmp = await _printTemplateService.GetDefaultTemplate(type);

                if (printTmp == null)
                    throw new Exception("Không tìm thấy mẫu in có sẵn");

                var paperSize = await _modelData.GetRef<PrintPaperSize>("base.paperformat_a4");

                printConfig = new PrintTemplateConfig();
                printConfig.Content = printTmp.Content;
                printConfig.Type = printTmp.Type;
                printConfig.PrintTemplate = printTmp;
                printConfig.PrintTemplateId = printTmp.Id;
                printConfig.PrintPaperSizeId = paperSize.Id;
                printConfig.PrintPaperSize = paperSize;
            }

            return printConfig;
        }

        public async Task<PrintTemplateConfig> ChangePaperSize(string type, Guid paperSizeId)
        {
            var printConfig = await SearchQuery(x => x.Type == type && x.PrintPaperSizeId == paperSizeId)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.PrintTemplate)
                .FirstOrDefaultAsync();

            if (printConfig == null)
            {
                var templateObj = GetService<IPrintTemplateService>();
                var printTmp = await _printTemplateService.GetDefaultTemplate(type);

                if (printTmp == null)
                    throw new Exception("Không tìm thấy mẫu in có sẵn");

                var paperSize = await _printPaperSizeService.SearchQuery(x => x.Id == paperSizeId).FirstOrDefaultAsync();

                printConfig = new PrintTemplateConfig();
                printConfig.Content = printTmp.Content;
                printConfig.Type = printTmp.Type;
                printConfig.PrintTemplate = printTmp;
                printConfig.PrintTemplateId = printTmp.Id;
                printConfig.PrintPaperSizeId = paperSize.Id;
                printConfig.PrintPaperSize = paperSize;
            }

            return printConfig;
        }

        public string ConnectLayoutForContent(string layout, string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(layout);
            doc.DocumentNode.SelectSingleNode("//div[@class='container']").InnerHtml += content;
            var newHtml = doc.DocumentNode.OuterHtml;
            return newHtml;
        }

        public async Task<string> PrintTest(string content, Guid paperSizeId, object data)
        {
            var printPaperSize = await _printPaperSizeService.SearchQuery(x => x.Id == paperSizeId).FirstOrDefaultAsync();
            var layoutHtml = File.ReadAllText("PrintTemplate/Shared/Layout.html");
            var template = Template.Parse(layoutHtml);
            var renderLayout = await template.RenderAsync(new { o = printPaperSize });

            var renderContent = await RenderTemplate(data, content);

            var result = ConnectLayoutForContent(renderLayout, renderContent);
            return result;
        }

        public async Task<string> RenderTemplate(object data, string content)
        {
            var userObj = GetService<IUserService>();
            var user = await userObj.GetCurrentUser();

            var scriptObject = new ScriptObject();
            scriptObject.Add("o", data);
            scriptObject.Add("u", user);

            var context = new TemplateContext();
            context.PushCulture(CultureInfo.CurrentCulture);
            context.PushGlobal(scriptObject);

            var template = Template.Parse(content);
            var result = await template.RenderAsync(context);
            return result;
        }

        public async Task<object> GetSampleData(string type)
        {
            var companyObj = GetService<ICompanyService>();
            var company = await companyObj.SearchQuery(x => x.Id == CompanyId).Include(x => x.Partner).FirstOrDefaultAsync();
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, @"SampleData\print_template_data.json");
            using (var reader = new StreamReader(filePath))
            {
                var fileContent = reader.ReadToEnd();
                var sample_data = JsonConvert.DeserializeObject<List<SampleDataPrintTemplate>>(fileContent);
                var item = sample_data.Where(s => s.Type == type).FirstOrDefault();

                switch (type)
                {
                    case "tmp_sale_order":
                        {
                            var res = JsonConvert.DeserializeObject<SaleOrder>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_toathuoc":
                        {
                            var res = JsonConvert.DeserializeObject<ToaThuoc>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_labo_order":
                        {
                            var res = JsonConvert.DeserializeObject<LaboOrder>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_purchase_order":
                        {
                            var res = JsonConvert.DeserializeObject<PurchaseOrder>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_purchase_refund":
                        {
                            var res = JsonConvert.DeserializeObject<PurchaseOrder>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_medicine_order":
                        {
                            var res = JsonConvert.DeserializeObject<MedicineOrder>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_phieu_thu":
                        {
                            var res = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_phieu_chi":
                        {
                            var res = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_customer_debt":
                        {
                            var res = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_agent_commission":
                        {
                            var res = JsonConvert.DeserializeObject<PhieuThuChi>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_stock_picking_incoming":
                        {
                            var res = JsonConvert.DeserializeObject<StockPicking>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_stock_picking_outgoing":
                        {
                            var res = JsonConvert.DeserializeObject<StockPicking>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_stock_inventory":
                        {
                            var res = JsonConvert.DeserializeObject<StockInventory>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_salary_employee":
                        {
                            var res = JsonConvert.DeserializeObject<SalaryPayment>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_salary_advance":
                        {
                            var res = JsonConvert.DeserializeObject<SalaryPayment>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_salary":
                        {
                            var res = JsonConvert.DeserializeObject<HrPayslip>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_advisory":
                        {
                            var res = JsonConvert.DeserializeObject<Advisory>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_account_payment":
                        {
                            var res = JsonConvert.DeserializeObject<SaleOrderPayment>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_supplier_payment":
                        {
                            var res = JsonConvert.DeserializeObject<AccountPayment>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_partner_advance":
                        {
                            var res = JsonConvert.DeserializeObject<PartnerAdvance>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_partner_refund":
                        {
                            var res = JsonConvert.DeserializeObject<PartnerAdvance>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_quotation":
                        {
                            var res = JsonConvert.DeserializeObject<Quotation>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    case "tmp_supplier_payment_inbound":
                        {
                            var res = JsonConvert.DeserializeObject<AccountPayment>(item.Data.ToString());
                            res.Company = company;
                            return res;
                        }
                    default:
                        return null;
                }
            }
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
