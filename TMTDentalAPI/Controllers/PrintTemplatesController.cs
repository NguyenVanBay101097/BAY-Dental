using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintTemplatesController : BaseApiController
    {
        private readonly IPrintTemplateService _printTemplateService;
        private readonly IMapper _mapper;

        public PrintTemplatesController(IPrintTemplateService printTemplateService, IMapper mapper)
        {
            _printTemplateService = printTemplateService;
            _mapper = mapper;
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> GetPrintTemplateDefault(PrintTemplateDefault val)
        {
            if (!ModelState.IsValid || val == null)
                return BadRequest();

            var printTemplate = await _printTemplateService.GetDefaultTemplate(val.Type);
            var res = _mapper.Map<PrintTemplateBasic>(printTemplate);
            return Ok(res);
        }


    }
}
