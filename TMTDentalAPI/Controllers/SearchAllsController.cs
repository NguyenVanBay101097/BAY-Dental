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
        private readonly ICardCardService _cardCardService;
        private readonly IServiceCardCardService _serviceCardCardService;
        private readonly IMapper _mapper;

        public SearchAllsController(IMapper mapper, IPartnerService partnerService, ISaleOrderService saleOrderService,
            ICardCardService cardCardService,
            IServiceCardCardService serviceCardCardService)
        {
            _partnerService = partnerService;
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _cardCardService = cardCardService;
            _serviceCardCardService = serviceCardCardService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAll(GetAllFilterVM val)
        {
            if (string.IsNullOrEmpty(val.Search))
                return Ok(new List<SearchAllViewModel>());

            var results = new List<SearchAllViewModel>();
            var limit = 20;

            if (val.ResultSelection == "all" || val.ResultSelection == "customer")
            {
                var customerQuery = _partnerService.SearchQuery(x => x.Active && x.Customer);
                IQueryable<Guid> partnersByKeywords;

                partnersByKeywords =
                      from p in customerQuery
                      where p.Name.Contains(val.Search) || p.NameNoSign.Contains(val.Search)
                || p.Ref.Contains(val.Search) || p.Phone.Contains(val.Search)
                      select p.Id;

                partnersByKeywords = partnersByKeywords.Union(
                        from card in _cardCardService.SearchQuery()
                        where card.Barcode.Contains(val.Search) && card.PartnerId.HasValue
                        select card.PartnerId.Value
                    );

                partnersByKeywords = partnersByKeywords.Union(
                       from card in _serviceCardCardService.SearchQuery()
                       where card.Barcode.Contains(val.Search) && card.PartnerId.HasValue
                       select card.PartnerId.Value
                   );

                customerQuery = from a in customerQuery
                             join pbk in partnersByKeywords on a.Id equals pbk
                             select a;


                var customers = await customerQuery
                    .Select(x => new SearchAllViewModel { 
                        Id = x.Id,
                        Name = "KH: " + x.Name,
                        Type = "customer",
                        Note = "Mã KH: " + (!string.IsNullOrEmpty(x.Ref) ? x.Ref : string.Empty) + "<br/>" +
                        "ĐT: " + (!string.IsNullOrEmpty(x.Phone) ? x.Phone : string.Empty)
                    })
                    .Take(limit)
                    .ToListAsync();

                results = results.Concat(customers).ToList();
                if (results.Count >= limit)
                    return Ok(results);
            }

            if (val.ResultSelection == "all" || val.ResultSelection == "supplier")
            {
                var suppliers = await _partnerService.SearchQuery(x => x.Supplier == true && (x.Name.Contains(val.Search) ||
                x.NameNoSign.Contains(val.Search) || x.Ref.Contains(val.Search) || x.Phone.Contains(val.Search)))
                    .Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = "NCC: " + x.Name,
                        Type = "supplier",
                        Note = "Mã NCC: " + (!string.IsNullOrEmpty(x.Ref) ? x.Ref : string.Empty) + "<br/>" +
                        "ĐT: " + (!string.IsNullOrEmpty(x.Phone) ? x.Phone : string.Empty)
                    })
                    .Take(limit)
                    .ToListAsync();

                results = results.Concat(suppliers).ToList();
                if (results.Count >= limit)
                    return Ok(results);
            }

            if (val.ResultSelection == "all" || val.ResultSelection == "sale-order")
            {
                var saleOrders = await _saleOrderService.SearchQuery(x => x.Name.Contains(val.Search))
                    .Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = "Phiếu điều trị: " + x.Name,
                        Type = "sale-order",
                        Note = "KH: " + x.Partner.Name + "<br/>" +
                        "ĐT: " + (!string.IsNullOrEmpty(x.Partner.Phone) ? x.Partner.Phone : string.Empty) + "<br/>" +
                        "Trạng thái: " + (x.State == "sale" ? "Đang điều trị" : (x.State == "done" ? "Hoàn thành" : "Nháp"))
                    })
                    .Take(limit)
                    .ToListAsync();

                results = results.Concat(saleOrders).ToList();
                if (results.Count >= limit)
                    return Ok(results);
            }

            //var res = new List<SearchAllViewModel>();
            //var res1 = new List<SearchAllViewModel>();
            //var query1 = _saleOrderService.SearchQuery(x => true);
            //switch (val.ResultSelection)
            //{
            //    case "customer":
            //        res = await _partnerService.SearchAll(new PartnerPaged() { Search = val.Search, Limit= val.Limit, Customer = true });
            //        break;
            //    case "supplier":
            //        res = await _partnerService.SearchAll(new PartnerPaged() { Search = val.Search, Limit = val.Limit, Supplier = true });
            //        break;
            //    case "sale-order":
            //        res = await _saleOrderService.SearchAll(new SaleOrderPaged() { Limit = val.Limit, Search = val.Search });
            //        break;
            //    case "all":
            //        res = await _partnerService.SearchAll(new PartnerPaged() { Search = val.Search, Limit = val.Limit, isBoth = true }); ;

            //        if (res.Count < 20)
            //        {
            //            res1 = await _saleOrderService.SearchAll(new SaleOrderPaged() { Limit = val.Limit - res.Count, Search = val.Search });
            //            res.AddRange(res1);
            //        }
            //        break;
            //    default:
            //        break;
            //}

            return Ok(results);
        }
    }
}
