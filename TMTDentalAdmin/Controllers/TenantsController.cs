using System;
using System.Collections.Generic;
using System.IO;
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
using OfficeOpenXml;
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
        private readonly ITenantOldSaleOrderProcessUpdateService _tenantOldSaleOrderProcessUpdateService;
        private readonly IHttpClientFactory _httpClientFactory;

        public TenantsController(ITenantService tenantService,
            IMapper mapper, IUnitOfWorkAsync unitOfWork,
            UserManager<ApplicationAdminUser> userManager,
            IConfiguration configuration,
            IOptions<AdminAppSettings> appSettings,
            ITenantExtendHistoryService tenantExtendHistoryService,
            ITenantOldSaleOrderProcessUpdateService tenantOldSaleOrderProcessUpdateService,
            IHttpClientFactory httpClientFactory
            )
        {
            _tenantService = tenantService;
            _tenantExtendHistoryService = tenantExtendHistoryService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _appSettings = appSettings?.Value;
            _configuration = configuration;
            _tenantOldSaleOrderProcessUpdateService = tenantOldSaleOrderProcessUpdateService;
            _httpClientFactory = httpClientFactory;
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
            tenant.DateExpired = DateTime.Now.AddDays(15);
            tenant.Version = _appSettings.Version;
            await _tenantService.CreateAsync(tenant);

            //var tenantExtendHistory = new TenantExtendHistory();
            //tenantExtendHistory.TenantId = tenant.Id;
            //tenantExtendHistory.ExpirationDate = tenant.DateExpired.Value;
            //tenantExtendHistory.ActiveCompaniesNbr = tenant.ActiveCompaniesNbr;
            //await _tenantExtendHistoryService.CreateAsync(tenantExtendHistory);

            try
            {
                var db = val.Hostname;
                CatalogDbContext context = DbContextHelper.GetCatalogDbContext(db, _configuration);
                await context.Database.MigrateAsync();

                HttpResponseMessage response = null;
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient())
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

            await _unitOfWork.BeginTransactionAsync();
            tenant.ActiveCompaniesNbr = val.ActiveCompaniesNbr;
            tenant.DateExpired = val.DateExpired;
            await _tenantService.UpdateAsync(tenant);
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    var response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant");
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw new Exception($"Không thể clear cache tenant {e.Message}");
            }

            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExtendExpired(TenantExtendExpiredViewModel val)
        {
            if (null == val || !ModelState.IsValid)
                return BadRequest();

            var tenant = await _tenantService.GetByIdAsync(val.TenantId);
            var now = DateTime.Now;

            await _unitOfWork.BeginTransactionAsync();

            var history = new TenantExtendHistory
            {
                ActiveCompaniesNbr = val.ActiveCompaniesNbr,
                TenantId = tenant.Id,
                ExpirationDate = tenant.DateExpired ?? DateTime.Now
            };

            if (val.CheckOption == "time")
            {
                var dateExpire = tenant.DateExpired.HasValue ? tenant.DateExpired.Value : DateTime.Now;
                if (dateExpire < now)
                    dateExpire = now;

                if (val.LimitOption == "day")
                    dateExpire = dateExpire.AddDays(val.Limit);
                if (val.LimitOption == "month")
                    dateExpire = dateExpire.AddMonths(val.Limit);
                if (val.LimitOption == "year")
                    dateExpire = dateExpire.AddYears(val.Limit);

                history.ExpirationDate = dateExpire;
            }

            await _tenantExtendHistoryService.CreateAsync(history);

            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    var response = await client.GetAsync($"{_appSettings.Schema}://{tenant.Hostname}.{_appSettings.CatalogDomain}/api/Companies/ClearCacheTenant");
                    tenant.ActiveCompaniesNbr = history.ActiveCompaniesNbr;
                    tenant.DateExpired = history.ExpirationDate;
                    await _tenantService.UpdateAsync(tenant);
                }

                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                throw new Exception($"Không thể clear cache tenant {e.Message}");
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

        [HttpPost("[action]")]
        public async Task<IActionResult> ProcessUpdateOldSaleOrder(IEnumerable<Guid> ids)
        {
            if (ids == null || ids.Count() == 0)
                return BadRequest();
            var notSuccessUpdates = await _tenantOldSaleOrderProcessUpdateService.SearchQuery(x => ids.Contains(x.TenantId) && (x.State == "draft" || x.State == "exception")).ToListAsync();
            var successUpdates = await _tenantOldSaleOrderProcessUpdateService.SearchQuery(x => ids.Contains(x.TenantId) && x.State == "done").ToListAsync();
            var toCreateUpdateIds = ids.Except(successUpdates.Select(x => x.TenantId).Except(notSuccessUpdates.Select(x => x.TenantId).ToList()).ToList());

            var createUpdates = new List<TenantOldSaleOrderProcessUpdate>();
            foreach (var id in toCreateUpdateIds)
                createUpdates.Add(new TenantOldSaleOrderProcessUpdate { TenantId = id });

            await _tenantOldSaleOrderProcessUpdateService.CreateAsync(createUpdates);
            var processIds = (createUpdates.Concat(notSuccessUpdates)).Select(x => x.Id).Distinct().ToList();
            return NoContent();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExportExcel(TenantPaged val)
        {
            val.Limit = int.MaxValue;
            var stream = new MemoryStream();
            var data = await _tenantService.GetPagedResultAsync(val);
            byte[] fileContent;

            var gender_dict = new Dictionary<string, string>()
            {
                { "male", "Nam" },
                { "female", "Nữ" },
                { "other", "Khác" }
            };

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Ngày tạo";
                worksheet.Cells[1, 2].Value = "Khách hàng";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Điện thoại";
                worksheet.Cells[1, 5].Value = "Phòng khám";
                worksheet.Cells[1, 6].Value = "Tên miền";
                worksheet.Cells[1, 7].Value = "Ngày hết hạn";
                worksheet.Cells[1, 8].Value = "Số chi nhánh";
                worksheet.Cells[1, 9].Value = "Nguồn khách hàng";
                worksheet.Cells[1, 10].Value = "Địa chỉ";
                worksheet.Cells[1, 11].Value = "Người triển khai";
               

                worksheet.Cells["A1:K1"].Style.Font.Bold = true;

                var row = 2;
                foreach (var item in data.Items)
                {
                    worksheet.Cells[row, 1].Value = item.DateCreated;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.Email;
                    worksheet.Cells[row, 4].Value = item.Phone;
                    worksheet.Cells[row, 5].Value = item.CompanyName;
                    worksheet.Cells[row, 6].Value = item.Hostname;
                    worksheet.Cells[row, 7].Value = item.DateExpired;
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "d/m/yyyy";
                    worksheet.Cells[row, 8].Value = item.ActiveCompaniesNbr;
                    worksheet.Cells[row, 9].Value = item.CustomerSource;
                    worksheet.Cells[row, 10].Value = item.Address;
                    worksheet.Cells[row, 11].Value = item.EmployeeAdmin != null ? item.EmployeeAdmin.Name : "";

                    row++;
                }

                worksheet.Column(4).Style.Numberformat.Format = "@";
                worksheet.Cells.AutoFitColumns();

                package.Save();

                fileContent = stream.ToArray();
            }

            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            stream.Position = 0;

            return new FileContentResult(fileContent, mimeType);
        }
    }
}