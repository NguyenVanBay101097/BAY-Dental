using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyERP.Utilities;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class SaleOrderLinesController : BaseController
    {
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly IMapper _mapper;
        private readonly IDotKhamStepService _dotKhamStepService;
        private readonly IProductService _productService;
        private readonly IProductStepService _productStepService;

        public SaleOrderLinesController(IProductService productService, IProductStepService productStepService, ISaleOrderLineService saleOrderLineService, IMapper mapper, IDotKhamStepService dotKhamStepService)
        {
            _saleOrderLineService = saleOrderLineService;
            _mapper = mapper;
            _productStepService = productStepService;
            _productService = productService;
            _dotKhamStepService = dotKhamStepService;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var res = _mapper.ProjectTo<SaleOrderLineViewModel>(_saleOrderLineService.SearchQuery(x => true));
            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromODataUri] Guid key, SaleOrderLineDotKhamSave val)
        {
            if (val == null)
                return BadRequest();

            await _saleOrderLineService.UpdateDkByOrderLine(key, val);
            return NoContent();
        }

        [EnableQuery]
        public SingleResult<SaleOrderLineViewModel> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<SaleOrderLineViewModel>(_saleOrderLineService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTeethList([FromODataUri] Guid key)
        {
            //
            var line = await _saleOrderLineService.SearchQuery(x => x.Id == key).Select(x => new SaleOrderLineViewModel
            {
                Teeth = x.SaleOrderLineToothRels.Select(x => new ToothDisplay
                {
                    Id = x.ToothId,
                    Name = x.Tooth.Name
                })
            }).FirstOrDefaultAsync();  
            
            return Ok(line.Teeth);

        }
    }
}
