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
    [Route("publicApi/SaleOrderPayments")]
    [ApiController]
    [CheckTokenPublic]
    [AllowAnonymous]
    public class SaleOrderPaymentsPublicController : ControllerBase
    {
        private readonly ISaleOrderPaymentService _orderPaymentService;
        private readonly IMapper _mapper;

        public SaleOrderPaymentsPublicController(ISaleOrderPaymentService orderPaymentService, IMapper mapper)
        {
            _orderPaymentService = orderPaymentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid? orderId = null)
        {
            var orderPayments = await _orderPaymentService.SearchQuery(x => (!orderId.HasValue || x.OrderId == orderId))
                .Select(x => new SaleOrderPaymentPublic { 
                    Amount = x.Amount,
                    Date = x.Date,
                    Journals = x.JournalLines.Select(s => new SaleOrderPaymentPublicJournals { 
                        Amount = s.Amount,
                        JournalName = s.Journal.Name
                    }),
                    Lines = x.Lines.Select(s => new SaleOrderPaymentPublicLines { 
                        Amount = s.Amount,
                        ProductName = s.SaleOrderLine.Name
                    }),
                    Note = x.Note,
                    State = x.State
                }).ToListAsync();
            return Ok(orderPayments);
        }
    }
}
