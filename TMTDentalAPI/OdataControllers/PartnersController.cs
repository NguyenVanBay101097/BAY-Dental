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
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.OdataControllers
{
    public class PartnersController : BaseController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly ConnectionStrings _connectionStrings;
        private readonly AppTenant _tenant;
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IPartnerPartnerCategoryRelService _partnerCategoryRelService;
        private readonly ISaleOrderService _saleOrderService;

        public PartnersController(IPartnerService partnerService,
            IMapper mapper,
            IOptions<ConnectionStrings> connectionStrings,
            ISaleOrderService saleOrderService,
            ITenant<AppTenant> tenant,
            IPartnerCategoryService partnerCategoryService,
            IPartnerPartnerCategoryRelService partnerCategoryRelService)
        {
            _partnerService = partnerService;
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _connectionStrings = connectionStrings?.Value;
            _tenant = tenant?.Value;
            _partnerCategoryService = partnerCategoryService;
            _partnerCategoryRelService = partnerCategoryRelService;
        }

        [EnableQuery]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> Get()
        {
            var results = await _partnerService.GetViewModelsAsync();
            return Ok(results);
        }


        [EnableQuery]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public SingleResult<PartnerInfoVm> Get([FromODataUri] Guid key)
        {
            var results = _partnerService.SearchQuery(x => x.Id == key).Select(x => new PartnerInfoVm
            {
                Id = x.Id,
                BirthYear = x.BirthYear,
                DisplayName = x.DisplayName,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                WardName = x.WardName,
                Street = x.Street,
                Tags = x.PartnerPartnerCategoryRels.Select(s => new PartnerCategoryViewModel
                {
                    Id = s.CategoryId,
                    Name = s.Category.Name,
                    Color = s.Category.Color
                })
            });

            return SingleResult.Create(results);
        }

        [EnableQuery]
        [CheckAccess(Actions = "Basic.SaleOrder.Read")]
        public IActionResult GetSaleOrders([FromODataUri] Guid key)
        {
            var results = _saleOrderService.SearchQuery(x => x.PartnerId == key && (!x.IsQuotation.HasValue || x.IsQuotation == false)).Select(x => new SaleOrderViewModel
            {
                Id = x.Id,
                AmountTotal = x.AmountTotal,
                DateOrder = x.DateOrder,
                Name = x.Name,
                PartnerId = x.PartnerId,
                IsQuotation = x.IsQuotation,
                Residual = x.Residual,
                State = x.State
            });

            return Ok(results);
        }

        [HttpPut]
        [CheckAccess(Actions = "Basic.Partner.Update")]
        public IActionResult Put([FromODataUri] Guid key, PartnerViewModel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(value);
            }

            return NoContent();
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> GetView(ODataQueryOptions<GridPartnerViewModel> options, [FromQuery] IEnumerable<Guid> tagIds)
        {
            var results = await _partnerService.GetGridViewModelsAsync();
            if (tagIds != null && tagIds.Any())
            {
                //filter query
                var filterPartnerIds = await _partnerCategoryRelService.SearchQuery(x => tagIds.Contains(x.CategoryId)).Select(x => x.PartnerId).Distinct().ToListAsync();
                results = results.Where(x => filterPartnerIds.Contains(x.Id));
            }

            var partnerVM = options.ApplyTo(results) as IQueryable<GridPartnerViewModel>;
            var partnerVMList = partnerVM.ToList();

            var partnerIds = partnerVMList.Select(x => x.Id).ToList();
            var partnerCategRels = await _partnerCategoryRelService.SearchQuery(x => partnerIds.Contains(x.PartnerId)).Select(x => new
            {
                PartnerId = x.PartnerId,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                Color = x.Category.Color
            }).ToListAsync();

            var tagDict = partnerCategRels.GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.Select(s => new TagModel
            {
                Id = s.CategoryId,
                Name = s.CategoryName,
                Color = s.Color
            }).ToList());

            foreach (var item in partnerVMList)
            {
                if (!tagDict.ContainsKey(item.Id))
                    continue;
                item.Tags = JsonConvert.SerializeObject(tagDict[item.Id]);
            }

            return Ok(partnerVMList);
        }

        [HttpGet("[action]")]
        [CheckAccess(Actions = "Basic.Partner.Read")]
        public async Task<IActionResult> GetDisplay([FromODataUri] Guid key)
        {
            var result = await _partnerService.SearchQuery(x => x.Id == key).Select(x => new PartnerDisplay
            {
                Id = x.Id,
                Avatar = x.Avatar,
                BirthDay = x.BirthDay,
                BirthMonth = x.BirthMonth,
                BirthYear = x.BirthYear,
                CityName = x.CityName,
                DistrictName = x.DistrictName,
                WardName = x.WardName,
                City = new CitySimple { Code = x.CityCode, Name = x.CityName },
                District = new DistrictSimple { Code = x.DistrictCode, Name = x.DistrictName },
                Ward = new WardSimple { Code = x.WardCode, Name = x.WardName },
                Ref = x.Ref,
                Name = x.Name,
                Date = x.Date,
                Email = x.Email,
                Gender = x.Gender,
                Street = x.Street,
                JobTitle = x.JobTitle,
                Phone = x.Phone,
                MedicalHistory = x.MedicalHistory,
                Histories = x.PartnerHistoryRels.Select(x => new HistorySimple()
                {
                    Id = x.HistoryId,
                    Name = x.History.Name
                }),
                Categories = x.PartnerPartnerCategoryRels.Select(s => new PartnerCategoryBasic
                {
                    Id = s.CategoryId,
                    Name = s.Category.Name,
                    Color = s.Category.Color
                })
            }).FirstOrDefaultAsync();

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
