using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class TenantExtendHistoryService : AdminBaseService<TenantExtendHistory>, ITenantExtendHistoryService
    {
        private readonly ITenantService _tenantService;
        private readonly UpdateExpiredDateTenantService _updateExpiredDateTenantService;
        private readonly AdminAppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;
        public TenantExtendHistoryService(IOptions<AdminAppSettings> appSettings, IOptions<ConnectionStrings> connectionStrings, ITenantService tenantService, IAsyncRepository<TenantExtendHistory> repository, IHttpContextAccessor httpContextAccessor) : base(repository, httpContextAccessor)
        {
            _tenantService = tenantService;
            _connectionStrings = connectionStrings?.Value;
            _appSettings = appSettings?.Value;
            _updateExpiredDateTenantService = new UpdateExpiredDateTenantService(_appSettings);
        }

        public override async Task<TenantExtendHistory> CreateAsync(TenantExtendHistory entity)
        {
            var today = DateTime.Today;
            var model = await base.CreateAsync(entity);
            var tenant = await _tenantService.GetByIdAsync(model.TenantId);
            if (model.StartDate == today)
            {
                var tenants = new List<AppTenant>();
                tenants.Add(tenant);
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionStrings.TenantConnection);
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    try
                    {
                        conn.Open();
                        await _updateExpiredDateTenantService.ComputeTenant(tenants, conn);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

            }
            return model;
        }

        public async Task<TenantExtendHistory> CreateAsync(TenantExtendHistorySave val)
        {
            var tenantExtendHistory = await ComputeTenantExtendHistory(val);
            tenantExtendHistory = await CreateAsync(tenantExtendHistory);
            return tenantExtendHistory;
        }

        public async Task<TenantExtendHistory> ComputeTenantExtendHistory(TenantExtendHistorySave val)
        {
            var tenant = await _tenantService.GetByIdAsync(val.TenantId);
            var today = DateTime.Today;
            var tenantExtendHistory = new TenantExtendHistory();
            switch (val.CheckOption)
            {
                case "time":
                    tenantExtendHistory.StartDate = tenant.DateExpired.HasValue ? tenant.DateExpired.Value.AddDays(1) : throw new Exception($"Ngày hết tạn của tên miền{tenant.Hostname} bị null");
                    tenantExtendHistory.ActiveCompaniesNbr = val.ActiveCompaniesNbr;
                    tenantExtendHistory.TenantId = tenant.Id;
                    tenantExtendHistory.ExpirationDate = tenantExtendHistory.StartDate.AbsoluteEndOfDate();
                    switch (val.LimitOption)
                    {
                        case "day":
                            tenantExtendHistory.ExpirationDate =  tenantExtendHistory.ExpirationDate.AddDays(val.Limit);
                            break;
                        case "month":
                            tenantExtendHistory.ExpirationDate =  tenantExtendHistory.ExpirationDate.AddMonths(val.Limit);
                            break;
                        case "year":
                            tenantExtendHistory.ExpirationDate =  tenantExtendHistory.ExpirationDate.AddYears(val.Limit);
                            break;
                        default:
                            break;
                    }
                    break;
                case "company":
                    tenantExtendHistory.TenantId = tenant.Id;
                    tenantExtendHistory.StartDate = today;
                    tenantExtendHistory.ExpirationDate = tenant.DateExpired.HasValue ? tenant.DateExpired.Value.AddDays(1) : throw new Exception($"Ngày hết tạn của tên miền{tenant.Hostname} bị null");
                    tenantExtendHistory.ActiveCompaniesNbr = val.ActiveCompaniesNbr;
                    break;
                default:
                    break;
            }
            return tenantExtendHistory;
        }

    }
}
