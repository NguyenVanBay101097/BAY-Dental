using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchAllsController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IMapper _mapper;

        public SearchAllsController(IMapper mapper, IPartnerService partnerService, ISaleOrderService saleOrderService)
        {
            _partnerService = partnerService;
            _saleOrderService = saleOrderService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAll(GetAllFilterVM val)
        {
            var res = new List<SearchAllViewModel>();
            var res1 = new List<SearchAllViewModel>();
            var query1 = _saleOrderService.SearchQuery(x => true);
            switch (val.ResultSelection)
            {
                case "customer":
                    res = await _partnerService.SearchAll(new PartnerPaged() { Search = val.Search, Limit= val.Limit, Customer = true });
                    break;
                case "supplier":
                    res = await _partnerService.SearchAll(new PartnerPaged() { Search = val.Search, Limit = val.Limit, Supplier = true });
                    break;
                case "sale-order":
                    res = await _saleOrderService.SearchAll(new SaleOrderPaged() { Limit = val.Limit, Search = val.Search });
                    break;
                case "all":
                    res = await _partnerService.SearchAll(new PartnerPaged() { Search = val.Search, Limit = val.Limit, isBoth = true }); ;

                    if (res.Count < 20)
                    {
                        res1 = await _saleOrderService.SearchAll(new SaleOrderPaged() { Limit = val.Limit - res.Count, Search = val.Search });
                        res.AddRange(res1);
                    }
                    break;
                default:
                    break;
            }

            return Ok(res);
        }
    }
}
