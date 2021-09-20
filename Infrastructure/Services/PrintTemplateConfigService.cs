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
            var layout = File.ReadAllText("PrintTemplate/Shared/Layout.html");

            var renderContent = await RenderTemplate(data, content);
            var renderLayout = await RenderTemplate(printPaperSize, layout);

            var result = ConnectLayoutForContent(renderLayout, renderContent);

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
