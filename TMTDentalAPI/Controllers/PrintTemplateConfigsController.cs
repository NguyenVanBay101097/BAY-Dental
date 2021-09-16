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

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintTemplateConfigsController : BaseApiController
    {
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IPrintPaperSizeService _printPaperSizeService;
        private readonly IMapper _mapper;

        public PrintTemplateConfigsController(IPrintTemplateConfigService printTemplateConfigService,IPrintPaperSizeService printPaperSizeService ,IMapper mapper)
        {
            _printTemplateConfigService = printTemplateConfigService;
            _printPaperSizeService = printPaperSizeService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDisplay(string type)
        {
            if (!ModelState.IsValid || type == null)
                return BadRequest();
            var res = await _printTemplateConfigService.GetPrintTemplateConfig(type);
            var display = _mapper.Map<PrintTemplateConfigDisplay>(res);
            return Ok(display);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePareSize(PrintTemplateConfigChangePaperSize val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            var res = await _printTemplateConfigService.ChangePaperSize(val.Type , val.PrintPaperSizeId);
            var display = _mapper.Map<PrintTemplateConfigDisplay>(res);
            return Ok(display);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> CreateOrUpdate(PrintTemplateConfigSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var printConfigs = await _printTemplateConfigService.SearchQuery(x => x.Type == val.Type)
                 .Include(x => x.PrintPaperSize)
                 .Include(x => x.Company)
                 .ToListAsync();

            if (printConfigs.Any())
            {
                var paperSize = await _printPaperSizeService.SearchQuery(x => x.Id == val.PrintPaperSizeId).FirstOrDefaultAsync();
                var tmpConfig = printConfigs.Where(x => x.PrintPaperSize.PaperFormat == paperSize.PaperFormat).FirstOrDefault();

                if (tmpConfig == null)
                {
                    var res = _mapper.Map<PrintTemplateConfig>(val);
                    await _printTemplateConfigService.CreateAsync(res);
                }

                var rs = _mapper.Map(val, tmpConfig);
                await _printTemplateConfigService.UpdateAsync(rs);

                foreach (var printConfig in printConfigs)
                    printConfig.IsDefault = printConfig.PrintPaperSize.PaperFormat == paperSize.PaperFormat ? true : false;

                await _printTemplateConfigService.UpdateAsync(printConfigs);

            }
            else
            {
                var res = _mapper.Map<PrintTemplateConfig>(val);
                res.IsDefault = true;
                await _printTemplateConfigService.CreateAsync(res);
            }

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

            var result = await _printTemplateConfigService.PrintTest(val.Content, val.Type, val.PrintPaperSizeId);
            return Ok(result);

        }
    }
}
