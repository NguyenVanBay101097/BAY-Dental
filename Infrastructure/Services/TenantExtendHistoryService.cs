using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
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

    }
}
