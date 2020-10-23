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
using Microsoft.AspNetCore.Http;
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
    public class PartnersController : BaseController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly ConnectionStrings _connectionStrings;
        private readonly AppTenant _tenant;
        private readonly IPartnerCategoryService _partnerCategoryService;
        private readonly IPartnerPartnerCategoryRelService _partnerCategoryRelService;

        public PartnersController(IPartnerService partnerService,
            IMapper mapper,
            IOptions<ConnectionStrings> connectionStrings,
            ITenant<AppTenant> tenant,
            IPartnerCategoryService partnerCategoryService,
            IPartnerPartnerCategoryRelService partnerCategoryRelService)
        {
            _partnerService = partnerService;
            _mapper = mapper;
            _connectionStrings = connectionStrings?.Value;
            _tenant = tenant?.Value;
            _partnerCategoryService = partnerCategoryService;
            _partnerCategoryRelService = partnerCategoryRelService;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get(ODataQueryOptions<PartnerViewModel> options, [FromQuery] IEnumerable<Guid> tagIds)
        {
            var results = await _partnerService.GetViewModelsAsync();
            if (tagIds != null && tagIds.Any())
                results = results.Where(x => x.Tags.Any(s => tagIds.Contains(s.Id)));
            return Ok(results);
        }

        [HttpPut]
        public IActionResult Put([FromODataUri]Guid key, PartnerViewModel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(value);
            }
           
            return NoContent();
        }

        [HttpGet("[action]")]
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
                CategoryName = x.Category.Name
            }).ToListAsync();

            var tagDict = partnerCategRels.GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.Select(s => new TagModel
            { 
                Id = s.CategoryId,
                Name = s.CategoryName
            }).ToList());

            foreach(var item in partnerVMList)
            {
                if (!tagDict.ContainsKey(item.Id))
                    continue;
                item.Tags = JsonConvert.SerializeObject(tagDict[item.Id]);
            }

            return Ok(partnerVMList);
        }
    }
}
