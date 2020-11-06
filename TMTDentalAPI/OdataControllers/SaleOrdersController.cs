using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class SaleOrdersController : BaseController
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleOrdersController(ISaleOrderService saleOrderService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var results = _mapper.ProjectTo<SaleOrderViewModel>(_saleOrderService.SearchQuery());
            return Ok(results);
        }

        [EnableQuery]
        public SingleResult<SaleOrderViewModel> Get([FromODataUri] Guid key)
        {
            var results = _mapper.ProjectTo<SaleOrderViewModel>(_saleOrderService.SearchQuery(x => x.Id == key));
            return SingleResult.Create(results);
        }

        public async Task<IActionResult> Post(SaleOrderSave model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _unitOfWork.BeginTransactionAsync();
            var order = await _saleOrderService.CreateOrderAsync(model);
            _unitOfWork.Commit();

            var basic = _mapper.Map<SaleOrderViewModel>(order);
            return Created(basic);
        }
    }
}
