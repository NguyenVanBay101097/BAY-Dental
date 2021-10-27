using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.PublicApiControllers
{
    [Route("publicApi/SaleOrderLines")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class SaleOrderLinesPublicController : ControllerBase
    {
        private readonly ISaleOrderLineService _orderLineService;
        private readonly IMapper _mapper;

        public SaleOrderLinesPublicController(ISaleOrderLineService orderLineService, IMapper mapper)
        {
            _orderLineService = orderLineService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? orderId = null)
        {
            var lines = await _orderLineService.SearchQuery(x => (!orderId.HasValue || x.OrderId == orderId.Value))
                .Select(x => new SaleOrderLinePublic { 
                    ProductName = x.Product.Name,
                    Date = x.Date,
                    State = x.State,
                    Teeth = x.SaleOrderLineToothRels.Select(s => s.Tooth.Name),
                    Diagnostic = x.Diagnostic,
                    DoctorName = x.Employee.Name,
                    AssistantName = x.Assistant.Name,
                    CounselorName = x.Counselor.Name,
                    PriceUnit = x.PriceUnit,
                    ProductUOMQty = x.ProductUOMQty,
                    PriceDiscountTotal = x.AmountDiscountTotal,
                    AmountInvoiced = x.AmountInvoiced,
                    PriceTotal = x.PriceTotal,
                    ToothType = x.ToothType
                }).ToListAsync();

            return Ok(lines);
        }
    }
}
