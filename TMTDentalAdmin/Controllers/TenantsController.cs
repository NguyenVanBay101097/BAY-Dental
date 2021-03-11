﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using AutoMapper;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Helpers;
using Infrastructure.Services;
using Infrastructure.TenantData;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly IConfiguration _configuration;
        private readonly ITenantExtendHistoryService _tenantExtendHistoryService;
        public TenantsController(ITenantService tenantService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork,
            UserManager<ApplicationAdminUser> userManager,
            IConfiguration configuration,
            IOptions<AdminAppSettings> appSettings,
            ITenantExtendHistoryService tenantExtendHistoryService
            )
        {
            _tenantService = tenantService;
            _tenantExtendHistoryService = tenantExtendHistoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _appSettings = appSettings?.Value;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TenantPaged val)
        {
            var result = await _tenantService.GetPagedResultAsync(val);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _tenantService.GetDisplay(id);
            return Ok(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(TenantRegisterViewModel val)
        {
            //Khi đăng ký gọi api sang app -> app check xem bên tenant có đăng ký hay chưa? Nếu có thì tạo database và setup tenant
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var tenant = _tenantService.SearchQuery(x => x.Hostname == val.Hostname).FirstOrDefault();
            if (tenant != null)
                throw new Exception("Địa chỉ gian hàng đã được sử dụng");

            tenant = _mapper.Map<AppTenant>(val);
            tenant.DateExpired = DateTime.Today.AddDays(15);
            await _tenantService.CreateAsync(tenant);

            var tenantExtendHistory = new TenantExtendHistory();
            tenantExtendHistory.TenantId = tenant.Id;
            tenantExtendHistory.StartDate = DateTime.Today;
            tenantExtendHistory.ExpirationDate = tenant.DateExpired.Value;
            tenantExtendHistory.ActiveCompaniesNbr = tenant.ActiveCompaniesNbr;
            await _tenantExtendHistoryService.CreateAsync(tenantExtendHistory);

            try
            {
                var db = val.Hostname;
                CatalogDbContext context = DbContextHelper.GetCatalogDbContext(db, _configuration);
                await context.Database.MigrateAsync();

                HttpResponseMessage response = null;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient(new RetryHandler(clientHandler)))
                {
                    response = await client.PostAsJsonAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/companies/setuptenant", new
                    {
                        CompanyName = val.CompanyName,
                        Name = val.Name,
                        Username = val.Username,
                        Password = val.Password,
                        Phone = val.Phone,
                        Email = val.Email
                    });
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Đăng ký thất bại, vui lòng thử lại sau");
            }
            catch (Exception e)
            {
                await _tenantService.DeleteAsync(tenant);
                throw e;
            }

            return Ok(val);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateDateExpired(TenantUpdateDateExpiredViewModel val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var tenant = await _tenantService.GetByIdAsync(val.Id);
            var oldDateExpired = tenant.DateExpired;
            var oldActiveCompaniesNbr = tenant.ActiveCompaniesNbr;
            tenant.DateExpired = val.DateExpired;
            tenant.ActiveCompaniesNbr = val.ActiveCompaniesNbr;
            await _tenantService.UpdateAsync(tenant);

            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpResponseMessage response = null;
                using (var client = new HttpClient(new RetryHandler(clientHandler)))
                {
                    response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant");
                }

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Có lỗi xảy ra");
            }
            catch (Exception e)
            {
                tenant.DateExpired = oldDateExpired;
                tenant.ActiveCompaniesNbr = oldActiveCompaniesNbr;
                await _tenantService.UpdateAsync(tenant);
                throw e;
            }

            return NoContent();
        }

        [HttpPost("{id}/[action]")]
        public async Task<IActionResult> UpdateInfo(Guid id, TenantUpdateInfoViewModel val)
        {
            var tenant = await _tenantService.GetByIdAsync(id);
            tenant = _mapper.Map(val, tenant);
            await _tenantService.UpdateAsync(tenant);

            return NoContent();
        }
    }
}