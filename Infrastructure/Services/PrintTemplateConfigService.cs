using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PrintTemplateConfigService(IAsyncRepository<PrintTemplateConfig> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper, IPrintTemplateService printTemplateService, IPrintPaperSizeService printPaperSizeService,IWebHostEnvironment webHostEnvironment, IIRModelDataService modelData)
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
            _printTemplateService = printTemplateService;
            _printPaperSizeService = printPaperSizeService;
            _webHostEnvironment = webHostEnvironment;
            _modelData = modelData;
        }

        public async Task<PrintTemplateConfigDisplay> GetDisplay(PrintTemplateConfigChangeType val)
        {
            var printConfig = await SearchQuery(x => x.Type == val.Type && x.IsDefault)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.Company)
                .FirstOrDefaultAsync();

            var display = _mapper.Map<PrintTemplateConfigDisplay>(printConfig);

            if(display == null)
            {            
                var printTmp = await _printTemplateService.SearchQuery(x => x.Type == val.Type).FirstOrDefaultAsync();
                if(printTmp == null)
                {
                    throw new Exception("Không tìm thấy mẫu in có sẵn");
                }

                var paperSize = await _modelData.GetRef<PrintPaperSize>("base.paperformat_a4");
                if (paperSize == null)
                    throw new Exception("Không tìm thấy khổ giấy mặc định");

                display = new PrintTemplateConfigDisplay();
                display.Content = printTmp.Content;
                display.Type = printTmp.Type;
                display.PrintPaperSizeId = paperSize.Id;
                display.PrintPaperSize = _mapper.Map<PrintPaperSizeDisplay>(paperSize);
            }

            return display;
        }

        public async Task CreateOrUpdate(PrintTemplateConfigSave val)
        {
            var printConfigs = await SearchQuery(x => x.Type == val.Type)
                .Include(x => x.PrintPaperSize)
                .Include(x => x.Company)
                .ToListAsync();

            if(printConfigs.Any())
            {
                var paperSize = await _printPaperSizeService.SearchQuery(x => x.Id == val.PrintPaperSizeId).FirstOrDefaultAsync();
                var tmpConfig = printConfigs.Where(x => x.PrintPaperSize.PaperFormat == paperSize.PaperFormat).FirstOrDefault();

                if(tmpConfig == null)
                {
                    var res = _mapper.Map<PrintTemplateConfig>(val);
                    await CreateAsync(res);
                }

                var rs = _mapper.Map(val, tmpConfig);
                await UpdateAsync(rs);

                foreach (var printConfig in printConfigs)
                    printConfig.IsDefault = printConfig.PrintPaperSize.PaperFormat == paperSize.PaperFormat ? true : false;

                await UpdateAsync(printConfigs);

            }
            else
            {
                var res = _mapper.Map<PrintTemplateConfig>(val);
                res.IsDefault = true;
                await CreateAsync(res);
            }
          
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
                var item = sample_data.Where(s=> s.Type == type).FirstOrDefault();

                switch (type)
                {
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
                        //res_purchase_order.User.Name = "Nguyễn Văn A";
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
                    default:
                        break;
                }
            }



            return obj;
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
