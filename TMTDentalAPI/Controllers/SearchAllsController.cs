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
            var query = _partnerService.SearchQuery(x => true);
            var query1 = _saleOrderService.SearchQuery(x => true);
            switch (val.ResultSelection)
            {
                case "customer":
                    res = await query.Include("PartnerPartnerCategoryRels").Include("PartnerPartnerCategoryRels.Category").Where(x => x.Name.Contains(val.Search) && x.Customer == true).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = "[" + x.Ref + "]" + " " + x.Name,
                        Address = x.GetAddress(),
                        Phone = x.Phone,
                        Type = "customer",
                        Tags = x.PartnerPartnerCategoryRels != null && x.PartnerPartnerCategoryRels.Count > 0 ? _mapper.Map<List<PartnerCategoryBasic>>(x.PartnerPartnerCategoryRels) : new List<PartnerCategoryBasic>()
                    }).ToListAsync();
                    break;
                case "supplier":
                    res = await query.Where(x => x.Name.Contains(val.Search) && x.Supplier == true).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = "[" + x.Ref + "]" + " " + x.Name,
                        Address = x.GetAddress(),
                        Phone = x.Phone,
                        Type = "supplier",
                        Tags = x.PartnerPartnerCategoryRels != null && x.PartnerPartnerCategoryRels.Count > 0 ? _mapper.Map<List<PartnerCategoryBasic>>(x.PartnerPartnerCategoryRels) : new List<PartnerCategoryBasic>()
                    }).ToListAsync();
                    break;
                case "sale-order":
                    res = await query1.Include(x => x.Partner).Where(x => x.Partner.Name.Contains(val.Search)).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = "[" + x.Partner.Ref + "]" + " " + x.Partner.Name,
                        SaleOrderName = x.Name,
                        Address = x.Partner.GetAddress(),
                        Phone = x.Partner.Phone,
                        Type = "sale-order",
                        State = x.State
                    }).ToListAsync();
                    break;
                case "all":

                    res = await query.Include("PartnerPartnerCategoryRels").Include("PartnerPartnerCategoryRels.Category").Where(x => x.Name.Contains(val.Search) && (x.Customer == true || x.Supplier == true)).Take(val.Limit).Select(x => new SearchAllViewModel
                    {
                        Id = x.Id,
                        Name = "[" + x.Ref + "]" + " " + x.Name,
                        Address = x.GetAddress(),
                        Phone = x.Phone,
                        Type = x.Customer ? "customer" : "supplier",
                        Tags = x.PartnerPartnerCategoryRels != null && x.PartnerPartnerCategoryRels.Count > 0 ? _mapper.Map<List<PartnerCategoryBasic>>(x.PartnerPartnerCategoryRels) : new List<PartnerCategoryBasic>()
                    }).ToListAsync();

                    if (res.Count < 20)
                    {
                        res1 = await query1.Include(x => x.Partner).Where(x => x.Partner.Name.Contains(val.Search)).Take(val.Limit - res.Count()).Select(x => new SearchAllViewModel
                        {
                            Id = x.Id,
                            SaleOrderName = x.Name,
                            Name = "[" + x.Partner.Ref + "]" + " " + x.Partner.Name,
                            Address = x.Partner.GetAddress(),
                            Type = "sale-order",
                            Phone = x.Partner.Phone,
                            State = x.State
                        }).ToListAsync();
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
