using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintTemplateConfigsController : BaseApiController
    {
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IMapper _mapper;

        public PrintTemplateConfigsController(IPrintTemplateConfigService printTemplateConfigService , IMapper mapper)
        {
            _printTemplateConfigService = printTemplateConfigService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetDisplay(PrintTemplateConfigChangeType val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();
            var res = await _printTemplateConfigService.GetDisplay(val);
            return Ok(res);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> CreateOrUpdate(PrintTemplateConfigSave val)
        {
            if (!ModelState.IsValid)
                return BadRequest();

             await _printTemplateConfigService.CreateOrUpdate(val);
            return NoContent();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Generate(GenerateReq val)
        {
            object obj = await _printTemplateConfigService.getDataTest(val.Type);
            var template = Template.Parse(val.Content); 
            try
            {
                var result = template.Render(obj);
                return Ok(result);

            }
            catch (Exception e)
            {
                throw new Exception("Không thể chuyển đổi do sai cú pháp");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PrintTest(PrintTestReq val)
        {
            object obj = await _printTemplateConfigService.getDataTest(val.Type);
            var printTemplateConfig = await _printTemplateConfigService.GetDisplay(new PrintTemplateConfigChangeType() { Type = val.Type});
            var template = Template.Parse(printTemplateConfig.Content);
            try
            {
                var result = template.Render(obj);
                return Ok(result);

            }
            catch (Exception e)
            {
                throw new Exception("Không thể chuyển đổi do sai cú pháp");
            }
        }
    }
}
