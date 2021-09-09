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

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintTemplateConfigsController : BaseApiController
    {
        private readonly IPrintTemplateConfigService _printTemplateConfigService;
        private readonly IMapper _mapper;

        public PrintTemplateConfigsController(IPrintTemplateConfigService printTemplateConfigService, IMapper mapper)
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
            object obj = await _printTemplateConfigService.GetSampleData(val.Type);
            // Creates 
            var scriptObj = new ScriptObject();
            scriptObj.Import(obj);
            var context = new TemplateContext();
            context.PushGlobal(scriptObj);
            var template = Template.Parse(val.Content);
            
            try
            {
                var result = await template.RenderAsync(context);
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
            if (string.IsNullOrEmpty(val.Content))
                throw new Exception("Mẫu in rỗng , cấu hình mãu in để in thử");

            var result = await _printTemplateConfigService.PrintTest(val);
            return Ok(result);

        }
    }
}
