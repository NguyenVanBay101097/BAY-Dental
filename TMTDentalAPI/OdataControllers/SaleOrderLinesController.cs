using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class SaleOrderLinesController : ControllerBase
    {
        private readonly ISaleOrderLineService _saleOrderLineService;
        private readonly IMapper _mapper;

        public SaleOrderLinesController(ISaleOrderLineService saleOrderLineService, IMapper mapper)
        {
            _saleOrderLineService = saleOrderLineService;
            _mapper = mapper;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var res = _mapper.ProjectTo<SaleOrderLineViewModel>(_saleOrderLineService.SearchQuery(x => true));
            return Ok(res);
        }
            
        [HttpGet]
        public SingleResult<SaleOrderLineViewModel> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<SaleOrderLineViewModel>(_saleOrderLineService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }

        //[HttpPut]
        //public async Task<IActionResult> 
    }
}
