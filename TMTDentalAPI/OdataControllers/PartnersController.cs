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
            var a = options.ApplyTo(results) as IQueryable<PartnerViewModel>;
            var b = a.ToList();

            //timf nhan cua 10 phan tu trong a
            foreach(var item in b)
            {
            }

            if (tagIds != null && tagIds.Any())
                results = results.Where(x => x.Tags.Any(s => tagIds.Contains(s.Id)));
            return Ok(a);
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

        [EnableQuery]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetView(ODataQueryOptions<PartnerViewModel> options)
        {
            var results = await _partnerService.GetViewModelsAsync();

            var partnerVM = options.ApplyTo(results) as IQueryable<PartnerViewModel>;
            var partnerVMList = partnerVM.ToList();

            var partnerIds = partnerVMList.Select(x => x.Id).ToList();
            var partnerCategRels = await _partnerCategoryRelService.SearchQuery(x => partnerIds.Contains(x.PartnerId)).Select(x => new
            {
                PartnerId = x.PartnerId,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name
            }).ToListAsync();

            var tagDict = partnerCategRels.GroupBy(x => x.PartnerId).ToDictionary(x => x.Key, x => x.Select(s => new PartnerCategoryViewModel { 
                Id = s.CategoryId,
                Name = s.CategoryName
            }).ToList());

            foreach(var item in partnerVMList)
            {
                if (!tagDict.ContainsKey(item.Id))
                    continue;
                item.Tags = tagDict[item.Id];
            }

            //timf nhan cua 10 phan tu trong a
            //var cateObj = GetService<IPartnerCategoryService>();
            //var cateObj  = await _partnerCategoryService.SearchQuery(x => x.Id == id).Include(x => x.Parent).FirstOrDefaultAsync();

            //var db = _tenant != null ? _tenant.Hostname : "localhost";
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.CatalogConnection);
            //if (db != "localhost")
            //    builder["Database"] = $"TMTDentalCatalogDb__{db}";

            //using (var conn = new SqlConnection(builder.ConnectionString))
            //{
            //    try
            //    {
            //        conn.Open();
            //        foreach (var item in partnerVMList)
            //        {
            //            var tag = conn.Query<PartnerCategoryViewModel>(
            //                "SELECT pc.Id, pc.Name " + 
            //                "FROM PartnerCategories pc " + 
            //                $"WHERE pc.Id IN (" + 
            //                    "SELECT ppcr.CategoryId " + 
            //                    "FROM PartnerPartnerCategoryRel ppcr " + 
            //                    $"WHERE ppcr.PartnerId = '{item.Id}' " + 
            //                ") ").ToList();
            //            item.Tags = tag;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //}

            return Ok(partnerVMList);
        }
    }
}
