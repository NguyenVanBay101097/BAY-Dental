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
using Infrastructure.TenantData;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using TMTDentalAPI.JobFilters;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : BaseApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IUploadService _uploadService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly CatalogDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIRModelAccessService _modelAccessService;
        private readonly IMemoryCache _cache;
        private readonly AppTenant _tenant;
        private readonly IPartnerService _partnerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TenantDbContext _tenantDbContext;
        private readonly AppSettings _appSettings;

        public CompaniesController(ICompanyService companyService, IUploadService uploadService,
            IUnitOfWorkAsync unitOfWork,
            CatalogDbContext context,
            IMapper mapper, IIRModelAccessService modelAccessService,
            IMemoryCache cache, ITenant<AppTenant> tenant,
            UserManager<ApplicationUser> userManager, IPartnerService partnerService,
            TenantDbContext tenantDbContext, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _companyService = companyService;
            _uploadService = uploadService;
            _context = context;
            _mapper = mapper;
            _modelAccessService = modelAccessService;
            _cache = cache;
            _tenant = tenant?.Value;
            _userManager = userManager;
            _partnerService = partnerService;
            _tenantDbContext = tenantDbContext;
            _appSettings = appSettings?.Value;
        }

        [HttpGet]
        [CheckAccess(Actions = "System.Company.Read")]
        public async Task<IActionResult> Get([FromQuery] CompanyPaged val)
        {
            var res = await _companyService.GetPagedResultAsync(val);
            return Ok(res);
        }

        [HttpGet("{id}")]
        [CheckAccess(Actions = "System.Company.Read")]
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
        [CheckAccess(Actions = "System.Company.Create")]
        public async Task<IActionResult> Create(CompanyDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            var partner = new Partner
            {
                Name = val.Name,
                Customer = false,
                Supplier = false,
                Email = val.Email,
                Phone = val.Phone,
                Street = val.Street,
            };

            if (val.City != null)
            {
                partner.CityCode = val.City.Code;
                partner.CityName = val.City.Name;
            }

            if (val.District != null)
            {
                partner.DistrictCode = val.District.Code;
                partner.DistrictName = val.District.Name;
            }

            if (val.Ward != null)
            {
                partner.WardCode = val.Ward.Code;
                partner.WardName = val.Ward.Name;
            }

            await _partnerService.CreateAsync(partner);

            var company = _mapper.Map<Company>(val);
            company.PartnerId = partner.Id;
            company.Active = true;
         
            await _companyService.CreateAsync(company);
            //await SaveLogo(company, val);

            await _companyService.InsertCompanyData(company);

            var userRoot = await _userManager.Users.Where(x => x.IsUserRoot).Include(x => x.ResCompanyUsersRels).FirstOrDefaultAsync();
            if (userRoot != null)
            {
                userRoot.ResCompanyUsersRels.Add(new ResCompanyUsersRel { Company = company });
                await _userManager.UpdateAsync(userRoot);
            }

            _unitOfWork.Commit();

            val.Id = company.Id;
            return CreatedAtAction(nameof(Get), new { id = company.Id }, val);
        }

        [HttpPut("{id}")]
        [CheckAccess(Actions = "System.Company.Update")]
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

            //await SaveLogo(company, val);
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.UpdateAsync(company);
            _unitOfWork.Commit();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [CheckAccess(Actions = "System.Company.Delete")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null)
                return NotFound();

            await _unitOfWork.BeginTransactionAsync();
            await _companyService.Unlink(company);
            _unitOfWork.Commit();

            return NoContent();
        }


        [HttpGet("InsertAccountDataForCompany")]
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
            //nếu tenant = null thì báo lỗi
            //if (_tenant == null)
            //    return Ok(new { Success = false, message = "Tên miền chưa đăng ký" });
            ////nếu database đã có dữ liệu thì return false
            //if (_context.Companies.ToList().Count > 0)
            //    return Ok(new { Success = false, message = "Database đã có dữ liệu" });

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _companyService.SetupTenant(val);

                _unitOfWork.Commit();

                //var tenant = await _tenantDbContext.Tenants.Where(x => x.Hostname == _tenant.Hostname).FirstOrDefaultAsync();
                //tenant.Version = _appSettings.Version;
                //_tenantDbContext.SaveChanges();

                //_cache.Remove(_tenant.Hostname.ToLower());
            }
            catch(Exception e)
            {
                _unitOfWork.Rollback();
                return Ok(new { Success = false, message = e.Message });
            }

            return Ok(new { Success = true, message = "" });
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public IActionResult ClearCacheTenant()
        {
            if (_tenant != null)
                _cache.Remove(_tenant.Hostname.ToLower());
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> InsertSecurityData()
        {
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.InsertSecurityData();
            _unitOfWork.Commit();
            return Ok(true);
        }

        /// <summary>
        /// Đóng
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> ActionArchive(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.ActionArchive(ids);
            _unitOfWork.Commit();
            return Ok(true);
        }

        /// <summary>
        /// Mở
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> ActionUnArchive(IEnumerable<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            await _companyService.ActionUnArchive(ids);
            _unitOfWork.Commit();
            return Ok(true);
        }

        private async Task SaveLogo(Company company, CompanyDisplay val)
        {
            if (!string.IsNullOrEmpty(val.Logo) && ImageHelper.IsBase64String(val.Logo))
            {
                
                var fileName = Path.GetRandomFileName() + "." + ImageHelper.GetImageExtension(val.Logo);
                var uploadResult = await _uploadService.UploadBinaryAsync(val.Logo, fileName: fileName);
                if (uploadResult != null)
                    company.Logo = uploadResult.FileUrl;
            }
            else if (string.IsNullOrEmpty(val.Logo))
            {
                company.Logo = null;
            }
        }
    }
}