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
    public class SaleOrderLinesController : ControllerBase
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
            var line = await _saleOrderLineService.SearchQuery(x => x.Id == key).Include(x => x.DotKhamSteps).FirstOrDefaultAsync();
            var valDict = val.Steps.ToDictionary(x => x.Id, x => x);
            var dksAdd = new List<DotKhamStep>();
            var dksRemove = new List<DotKhamStep>();

            if (line == null)
                return NotFound();

            foreach (var item in line.DotKhamSteps.ToList())
            {
                if (!val.Steps.Any(x => x.Id == item.Id))
                {
                    dksRemove.Add(item);
                    line.DotKhamSteps.Remove(item);
                }

                if (valDict.ContainsKey(item.Id))
                {
                    item.Name = valDict[item.Id].Name;
                    item.Order = valDict[item.Id].Order;
                }
            }
            foreach (var item in val.Steps)
            {
                if (!line.DotKhamSteps.Any(x => x.Id == item.Id))
                {
                    dksAdd.Add(new DotKhamStep
                    {
                        Id = item.Id,
                        IsDone = item.IsDone,
                        Name = item.Name,
                        Order = item.Order,
                        ProductId = line.ProductId,
                        SaleLineId = line.Id,
                        SaleOrderId = line.OrderId
                    });
                }
            }

            await _dotKhamStepService.CreateAsync(dksAdd);
            await _dotKhamStepService.DeleteAsync(dksRemove);
            await _dotKhamStepService.UpdateAsync(line.DotKhamSteps.ToList());
            if (val.Default)
            {
                var service = await _productService.SearchQuery(x => x.Id == line.ProductId).Include(x => x.Steps).FirstOrDefaultAsync();
                var stepsAdd = new List<ProductStep>();
                foreach (var item in val.Steps)
                {
                    stepsAdd.Add(new ProductStep
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Order = item.Order,
                        Active = true,
                        Default = true,
                        ProductId = service.Id,
                    });
                }
                await _productStepService.DeleteAsync(service.Steps.ToList());
                await _productStepService.CreateAsync(stepsAdd);
            }

            return NoContent();
        }

        [EnableQuery]
        public SingleResult<SaleOrderLineViewModel> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<SaleOrderLineViewModel>(_saleOrderLineService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }
    }
}
