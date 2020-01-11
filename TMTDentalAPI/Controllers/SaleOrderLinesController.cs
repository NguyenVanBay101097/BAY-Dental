﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Models;
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
    public class SaleOrderLinesController : BaseApiController
    {
        private readonly ISaleOrderLineService _saleLineService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;

        public SaleOrderLinesController(ISaleOrderLineService saleLineService, IMapper mapper,
            IUnitOfWorkAsync unitOfWork)
        {
            _saleLineService = saleLineService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("OnChangeProduct")]
        public async Task<IActionResult> OnChangeProduct(SaleOrderLineOnChangeProduct val)
        {
            var res = await _saleLineService.OnChangeProduct(val);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SaleOrderLinesPaged val)
        {
            var result = await _saleLineService.GetPagedResultAsync(val);

            var paged = new PagedResult2<SaleOrderLineBasic>(result.TotalItems, val.Offset, val.Limit)
            {
                //Có thể dùng thư viện automapper
                Items = _mapper.Map<IEnumerable<SaleOrderLineBasic>>(result.Items),
            };

            return Ok(paged);
        }
    }
}