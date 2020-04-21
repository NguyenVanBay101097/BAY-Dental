using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.TenantData;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Web.Models.ContentEditing;

namespace TMTDentalAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseApiController
    {
        private readonly ITenantService _tenantService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly UserManager<ApplicationAdminUser> _userManager;
        private readonly AdminAppSettings _appSettings;
        public TenantsController(ITenantService tenantService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork,
            UserManager<ApplicationAdminUser> userManager,
            IOptions<AdminAppSettings> appSettings)
        {
            _tenantService = tenantService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _appSettings = appSettings?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]TenantPaged val)
        {
            var result = await _tenantService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TenantDisplay val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            await _unitOfWork.BeginTransactionAsync();
            var tenant = _mapper.Map<AppTenant>(val);

            await _tenantService.CreateAsync(tenant);

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsJsonAsync($"http://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/companies/setuptenant", new {
                CompanyName = val.CompanyName,
                Name = val.Name,
                Username = "dangthetai",
                Password = "123123",
                Phone = val.Phone,
                Email = val.Email
            });

            _unitOfWork.Commit();

            val.Id = tenant.Id;
            return Ok(val);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(TenantRegisterViewModel val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var tenant = _tenantService.SearchQuery(x => x.Hostname == val.Hostname).FirstOrDefault();
            if (tenant != null)
                throw new Exception("Địa chỉ gian hàng đã được sử dụng");

            tenant = _mapper.Map<AppTenant>(val);
            await _tenantService.CreateAsync(tenant);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(1, 0, 0);
                HttpResponseMessage response = await client.PostAsJsonAsync($"http://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/companies/setuptenant", new
                {
                    CompanyName = val.CompanyName,
                    Name = val.Name,
                    Username = val.Username,
                    Password = val.Password,
                    Phone = val.Phone,
                    Email = val.Email
                });

                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var result = await JsonSerializer.DeserializeAsync<BaseHandleErrorVM>(responseStream);
                    if (!result.success)
                    {
                        await _tenantService.DeleteAsync(tenant);
                        throw new Exception(result.message);
                    }
                }
            }

            return NoContent();
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateDateExpired(TenantUpdateDateExpiredViewModel val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();
            await _unitOfWork.BeginTransactionAsync();
            var tenant = await _tenantService.GetByIdAsync(val.Id);
            tenant.DateExpired = val.DateExpired;
            await _tenantService.UpdateAsync(tenant);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(1, 0, 0);
                HttpResponseMessage response = await client.GetAsync($"http://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant?skipCheckExpired=true");
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Something fail");

                _unitOfWork.Commit();
            }

            return NoContent();
        }

    }
}