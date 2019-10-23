using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SaasKit.Multitenancy;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IUploadService _uploadService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IMemoryCache _cache;
        private readonly AppTenant _tenant;
        public CompaniesController(ICompanyService companyService, IUploadService uploadService,
            IUnitOfWorkAsync unitOfWork,
            CatalogDbContext context,
            IMapper mapper, IIRModelAccessService modelAccessService,
            IMemoryCache cache, ITenant<AppTenant> tenant)
        {
            _unitOfWork = unitOfWork;
            _companyService = companyService;
            _uploadService = uploadService;
            _context = context;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
            _cache = cache;
            _tenant = tenant?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CompanyPaged val)
        {
            var res = await _companyService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var company = await _companyService.SearchQuery(x => x.Id == id).Include(x => x.Partner).FirstOrDefaultAsync();
            if (company == null)
            {
                return NotFound();
            }
            var res = _mapper.Map<CompanyDisplay>(company);
            res.Street = company.Partner.Street;
            res.City = new CitySimple { Name = company.Partner.CityName, Code = company.Partner.CityCode };
            res.District = new DistrictSimple { Name = company.Partner.DistrictName, Code = company.Partner.DistrictCode };
            res.Ward = new WardSimple { Name = company.Partner.WardName, Code = company.Partner.WardCode };
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CompanyDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            _modelAccessService.Check("Company", "Create");
            var company = _mapper.Map<Company>(val);

            await _unitOfWork.BeginTransactionAsync();
            //await _companyService.CreateAsync(company);
            _unitOfWork.Commit();

            val.Id = company.Id;
            return CreatedAtAction(nameof(Get), new { id = company.Id }, val);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CompanyDisplay val)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var company = await _companyService.SearchQuery(x => x.Id == id).Include(x => x.Partner).FirstOrDefaultAsync();
            if (company == null)
                return NotFound();

            company = _mapper.Map(val, company);
            company.Partner.Street = val.Street;
            company.Partner.CityCode = val.City != null ? val.City.Code : string.Empty;
            company.Partner.CityName = val.City != null ? val.City.Name : string.Empty;
            company.Partner.DistrictCode = val.District != null ? val.District.Code : string.Empty;
            company.Partner.DistrictName = val.District != null ? val.District.Name : string.Empty;
            company.Partner.WardCode = val.Ward != null ? val.Ward.Code : string.Empty;
            company.Partner.WardName = val.Ward != null ? val.Ward.Name : string.Empty;

            await SaveLogo(company, val);
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.UpdateAsync(company);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
                return NotFound();
            await _companyService.DeleteAsync(company);

            return NoContent();
        }


        [HttpPost("InsertAccountDataForCompany")]
        public async Task<IActionResult> InsertAccountDataForCompany()
        {
            await _unitOfWork.BeginTransactionAsync();
            var company = await _companyService.SearchQuery().FirstOrDefaultAsync();
            await _companyService.InsertModuleAccountData(company);
            _unitOfWork.Commit();
            return Ok(true);
        }

        [HttpPost("InsertStockDataForCompany")]
        public async Task<IActionResult> InsertStockDataForCompany()
        {
            await _unitOfWork.BeginTransactionAsync();
            var company = await _companyService.SearchQuery().FirstOrDefaultAsync();
            await _companyService.InsertModuleStockData(company);
            _unitOfWork.Commit();
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpPost("SetupTenant")]
        public async Task<IActionResult> SetupTenant(CompanySetupTenant val)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.SetupTenant(val);
            _unitOfWork.Commit();
          
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult ClearCacheTenant()
        {
            if (_tenant != null)
                _cache.Remove(_tenant.Hostname.ToLower());
            return Ok(true);
        }

     

        [HttpPost("[action]")]
        public async Task<IActionResult> InsertSecurityData()
        {
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.InsertSecurityData();
            _unitOfWork.Commit();
            return Ok(true);
        }

        private async Task SaveLogo(Company company, CompanyDisplay val)
        {
            if (!string.IsNullOrEmpty(val.Logo) && ImageHelper.IsBase64String(val.Logo))
            {
                var fileName = Path.GetRandomFileName() + "." + ImageHelper.GetImageExtension(val.Logo);
                company.Logo = await _uploadService.UploadBinaryAsync(val.Logo, fileName: fileName);
            }
            else if (string.IsNullOrEmpty(val.Logo))
            {
                company.Logo = null;
            }
        }
    }
}