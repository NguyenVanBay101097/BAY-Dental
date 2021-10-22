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
            object data = await _printTemplateConfigService.GetSampleData(val.Type);
            var result = await _printTemplateConfigService.RenderTemplate(data, val.Content);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PrintTest(PrintTestReq val)
        {
            if (string.IsNullOrEmpty(val.Content))
                throw new Exception("Mẫu in rỗng , cấu hình mãu in để in thử");
            object data = await _printTemplateConfigService.GetSampleData(val.Type);
            var result = await _printTemplateConfigService.PrintTest(val.Content, val.PrintPaperSizeId, data);
            return Ok(result);

        }

      
    }
}
