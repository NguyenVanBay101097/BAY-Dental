using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigPrintsController : BaseApiController
    {
        private readonly IConfigPrintService _configPrintService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public ConfigPrintsController(IConfigPrintService configPrintService, IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _configPrintService = configPrintService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _configPrintService.LoadConfigPrint();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUpdateConfigPrint(IEnumerable<ConfigPrintSave> vals)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            await _configPrintService.CreateUpdateConfigPrint(vals);
            _unitOfWork.Commit();

            return NoContent();
        }
    }
}
