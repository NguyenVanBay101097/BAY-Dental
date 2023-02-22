using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockPickingTypesController : BaseApiController
    {
        private readonly IStockPickingTypeService _pickingTypeService;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;

        public StockPickingTypesController(IStockPickingTypeService pickingTypeService,
            IMapper mapper, IIRModelAccessService modelAccessService)
        {
            _pickingTypeService = pickingTypeService;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]StockPickingTypePaged val)
        {
            var res = await _pickingTypeService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}/GetBasic")]
        public async Task<IActionResult> GetBasic(Guid id)
        {
            var pickingType = await _pickingTypeService.GetByIdAsync(id);
            var res = _mapper.Map<StockPickingTypeBasic>(pickingType);
            return Ok(res);
        }
    }
}