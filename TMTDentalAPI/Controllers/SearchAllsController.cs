using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public SearchAllsController(IPartnerService partnerService, ISaleOrderService saleOrderService)
        {
            _partnerService = partnerService;
            _saleOrderService = saleOrderService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAll(GetAllFilterVM val)
        {
            var res = new List<SearchAllViewModel>();
            var query = _partnerService.SearchQuery(x => true);
            var query1 = _saleOrderService.SearchQuery(x => true);
            switch (val.ResultSelection)
            {
                case "customer":
                    res = await query.Include("PartnerPartnerCategoryRels").Include("PartnerPartnerCategoryRels.Category").Where(x => x.Name.Contains(val.Search) && x.Customer == true).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.GetAddress(),
                        Phone = x.Phone,
                        Tags = x.PartnerPartnerCategoryRels != null && x.PartnerPartnerCategoryRels.Count > 0 ? x.PartnerPartnerCategoryRels.Select(s => s.Category.Name).ToList() : new List<string>()
                    }).ToListAsync();
                    break;
                case "supplier":
                    res = await query.Where(x => x.Name.Contains(val.Search) && x.Supplier == true).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.GetAddress(),
                        Phone = x.Phone,
                        Tags = x.PartnerPartnerCategoryRels.Select(s => s.Category.Name).ToList()
                    }).ToListAsync();
                    break;
                case "sale-order":
                    res = await query1.Include(x => x.Partner).Where(x => x.Partner.Name.Contains(val.Search)).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = x.Partner.Name,
                        Address = x.Partner.GetAddress(),
                        Phone = x.Partner.Phone,
                        State = x.State
                    }).ToListAsync();
                    break;
                case "all":

                    break;
                default:
                    break;
            }

            return Ok(res);
        }
    }
}
